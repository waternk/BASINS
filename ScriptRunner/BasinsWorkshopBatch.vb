﻿Imports System.Collections.Specialized
Imports MapWindow.Interfaces
Imports MapWinUtility

Imports atcUtility
Imports atcData
Imports BASINS
Imports D4EMDataManager

Module BasinsWorkshopBatch
    Private pMapWin As IMapWin

    Public Sub ScriptMain(ByRef aMapWin As IMapWin)
        pMapWin = aMapWin
        Dim lOriginalDir As String = IO.Directory.GetCurrentDirectory
        Dim lOriginalDisplayMessages As Boolean = Logger.DisplayMessageBoxes
        Logger.DisplayMessageBoxes = False
        Logger.Dbg("BasinsWorkshopBatch:CurDir:" & lOriginalDir)
        Dim lBasinsProjectDataDir As String = DefaultBasinsDataDir() & "WorkshopBatch\"
        Try
            If IO.Directory.Exists(lBasinsProjectDataDir) Then
                IO.Directory.Delete(lBasinsProjectDataDir, True)
            End If
            If Not Exercise1(lBasinsProjectDataDir) Then
                Logger.Dbg("***** Exercise1 FAIL *****")
            ElseIf Not Exercise2() Then
                Logger.Dbg("***** Exercise2 FAIL *****")
            End If
        Catch lEx As Exception
            Logger.Dbg("Problem " & lEx.ToString)
        End Try
        IO.Directory.SetCurrentDirectory(lOriginalDir)
        Logger.DisplayMessageBoxes = lOriginalDisplayMessages
    End Sub

    Private Function Exercise1(ByVal aBasinsProjectDataDir As String) As Boolean
        'Adding Data to a New BASINS Project
        '  Build a New BASINS Project
        '  TODO: open National Project and select Patuxent in code
        '  TODO: clear cache?
        Dim lProjection As String = "proj +proj=utm +zone=18 +ellps=GRS80 +lon_0=-75 +lat_0=0 +k=0.9996 +x_0=500000.0 +y_0=0 end "
        SaveFileString(aBasinsProjectDataDir & "prj.proj", lProjection) 'Side effect: makes data directory
        Dim lRegion As String = _
           "<region>" & _
           "   <northbc>1975392.91047589</northbc>" & _
           "   <southbc>1866978.51560156</southbc>" & _
           "   <eastbc>1684619.12695581</eastbc>" & _
           "   <westbc>1595425.21946512</westbc>" & _
           "   <projection>+proj=aea +lat_1=29.5 +lat_2=45.5 +lat_0=23 +lon_0=-96 +x_0=0 +y_0=0 +ellps=GRS80 +datum=NAD83 +units=m +no_defs</projection>" & _
           "   <HUC8 status=""set by BASINS System Application"">02060006</HUC8>" & _
           "</region> "
        CreateNewProjectAndDownloadCoreData(lRegion, DefaultBasinsDataDir, aBasinsProjectDataDir, aBasinsProjectDataDir & "Patuxent.mwprj")
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        '  Navigate the BASINS 4.0 GIS Environment

        '  General Data Download
        Dim lPlugins As New ArrayList
        For lPluginIndex As Integer = 0 To pMapWin.Plugins.Count
            Try
                If Not pMapWin.Plugins.Item(lPluginIndex) Is Nothing Then
                    lPlugins.Add(pMapWin.Plugins.Item(lPluginIndex))
                End If
            Catch lEx As Exception
                Debug.Print(lPluginIndex)
            End Try
        Next
        Dim lDownloadManager As New D4EMDataManager.DataManager(lPlugins)
        Dim lQuery As String = _
          "<function name='#Name#'>" & _
          "  <arguments>" & _
          "    <DataType>#DataType#</DataType>" & _
          "    <SaveIn>C:\Basins\data\WorkshopBatch</SaveIn>" & _
          "    <CacheFolder>C:\BASINS\cache\</CacheFolder>" & _
          "    <DesiredProjection>+proj=utm +zone=18 +ellps=GRS80 +datum=NAD83 +units=m +no_defs</DesiredProjection>" & _
          "    <region>" & _
          "       <northbc>39.3668056784273</northbc>" & _
          "       <southbc>38.257273329737</southbc>" & _
          "       <eastbc>-76.4009709099128</eastbc>" & _
          "       <westbc>-77.2070255407762</westbc>" & _
          "       <HUC8>02060006</HUC8>" & _
          "       <preferredformat>huc8</preferredformat>" & _
          "       <projection>+proj=latlong +datum=NAD83</projection>" & _
          "    </region>" & _
          "    <clip>False</clip>" & _
          "    <merge>False</merge>" & _
          "    <joinattributes>true</joinattributes>" & _
          "  </arguments>" & _
          "</function>"

        '  Add LandUse Data
        Dim lQueryLU As String = lQuery.Replace("#DataType#", "Giras").Replace("#Name#", "GetBASINS")
        Dim lResultLU As String = lDownloadManager.Execute(lQueryLU)
        If lResultLU Is Nothing OrElse lResultLU.Length = 0 Then
            'Nothing to report, no success or error
        ElseIf lResultLU.StartsWith("<success>") Then
            BASINS.ProcessDownloadResults(lResultLU)
            For lLayerIndex As Integer = 0 To pMapWin.Layers.NumLayers - 1
                With pMapWin.Layers(lLayerIndex)
                    If .Name.ToLower.Contains("land use") Then
                        .Visible = False
                    End If
                End With
            Next
        Else
            Logger.Msg(atcUtility.ReadableFromXML(lResultLU), "LUDownload Result")
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        '  Add NHDPlus Data
        Dim lQueryNHD As String = lQuery.Replace("#DataType#", "hydrography</DataType> <DataType>Catchment").Replace("#Name#", "GetNHDPlus")
        Dim lResultNHD As String = lDownloadManager.Execute(lQueryNHD)
        If lResultNHD Is Nothing OrElse lResultNHD.Length = 0 Then
            'Nothing to report, no success or error
        ElseIf lResultNHD.StartsWith("<success>") Then
            BASINS.ProcessDownloadResults(lResultNHD)
            For Each lLayer As MapWindow.Interfaces.Layer In pMapWin.Layers
                Dim lName As String = lLayer.Name.ToLower
                If lName.Contains("flowline") OrElse _
                   lName.Contains("catchment") OrElse _
                   lName.Contains("area features") OrElse _
                   lName.Contains("waterbody features") Then
                    lLayer.Visible = False
                End If
            Next
        Else
            Logger.Msg(atcUtility.ReadableFromXML(lResultNHD), "NHDDownload Result")
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        '  Add BASINS census and TIGER line data
        Dim lQueryCensus As String = lQuery.Replace("#DataType#", "Census").Replace("#Name#", "GetBASINS")
        Dim lResultCensus As String = lDownloadManager.Execute(lQueryCensus)
        If lResultCensus Is Nothing OrElse lResultCensus.Length = 0 Then
            'Nothing to report, no success or error
        ElseIf lResultCensus.StartsWith("<success>") Then
            BASINS.ProcessDownloadResults(lResultCensus)
            For Each lLayer As MapWindow.Interfaces.Layer In pMapWin.Layers
                If lLayer.Name.ToLower.Contains("tiger") Then
                    lLayer.Visible = False
                End If
            Next
        Else
            Logger.Msg(atcUtility.ReadableFromXML(lResultNHD), "CensusDownload Result")
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        '  Add BASINS Digital Elevation Model (DEM) grids
        Dim lQueryDEMG As String = lQuery.Replace("#DataType#", "DEMG").Replace("#Name#", "GetBASINS")
        Dim lResultDEMG As String = lDownloadManager.Execute(lQueryDEMG)
        If lResultDEMG Is Nothing OrElse lResultDEMG.Length = 0 Then
            'Nothing to report, no success or error
        ElseIf lResultDEMG.StartsWith("<success>") Then
            BASINS.ProcessDownloadResults(lResultDEMG)
        Else
            Logger.Msg(atcUtility.ReadableFromXML(lResultNHD), "DEMGDownload Result")
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        '  Import other shapefiles
        For Each lLayer As MapWindow.Interfaces.Layer In pMapWin.Layers
            If lLayer.Name.ToLower.Contains("flowline features") Then
                lLayer.Visible = True
                Exit For
            End If
        Next
        Dim lPreDefDelinFile As String = aBasinsProjectDataDir.Replace("WorkshopBatch\", "WorkshopFiles\Predefined Delineations\w_branch.shp")
        If IO.File.Exists(lPreDefDelinFile) Then
            Dim lLayerAdd As MapWindow.Interfaces.Layer = pMapWin.Layers.Add(lPreDefDelinFile)
            With lLayerAdd
                .OutlineColor = Drawing.Color.Red
                .DrawFill = False
            End With
        Else
            Logger.Dbg("FileNotFound:" & lPreDefDelinFile)
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        '  Download timeseries data for use in modeling
        Dim lQueryMetStations As String = lQuery.Replace("#DataType#", "MetStations").Replace("#Name#", "GetBASINS")
        Dim lResultMetStations As String = lDownloadManager.Execute(lQueryMetStations)
        If lResultMetStations Is Nothing OrElse lResultMetStations.Length = 0 Then
            'Nothing to report, no success or error
        ElseIf lResultMetStations.StartsWith("<success>") Then
            BASINS.ProcessDownloadResults(lResultMetStations)
        Else
            Logger.Msg(atcUtility.ReadableFromXML(lResultMetStations), "MetStationsDownload Result")
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        Dim lQueryMetData As String = "<function name='GetBASINS'> <arguments> <DataType>MetData</DataType> <SaveWDM>C:\Basins\data\WorkshopBatch\met\met.wdm</SaveWDM> <SaveIn>C:\Basins\data\WorkshopBatch</SaveIn> <CacheFolder>C:\dev\BASINS40\</CacheFolder> <DesiredProjection>+proj=utm +zone=18 +ellps=GRS80 +datum=NAD83 +units=m +no_defs</DesiredProjection> <region> <northbc>39.3668056784273</northbc> <southbc>38.257273329737</southbc> <eastbc>-76.4009709099128</eastbc> <westbc>-77.2070255407762</westbc> <HUC8>02060006</HUC8> <preferredformat>huc8</preferredformat> <projection>+proj=latlong +datum=NAD83</projection> </region> " & _
                                      "<stationid>MD180193</stationid> <stationid>MD180193</stationid> <stationid>MD180193</stationid> <stationid>MD180460</stationid> <stationid>MD180460</stationid> <stationid>MD180460</stationid> <stationid>MD180465</stationid> <stationid>MD180465</stationid> <stationid>MD180465</stationid> <stationid>MD180465</stationid> <stationid>MD180465</stationid> <stationid>MD180465</stationid> <stationid>MD180465</stationid> " & _
                                      "<stationid>MD180465</stationid> <stationid>MD180470</stationid> <stationid>MD180470</stationid> <stationid>MD180470</stationid> <stationid>MD180470</stationid> <stationid>MD180475</stationid> <stationid>MD180475</stationid> <stationid>MD180475</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180700</stationid> <stationid>MD180701</stationid> <stationid>MD180701</stationid> <stationid>MD180701</stationid> <stationid>MD180702</stationid> <stationid>MD180702</stationid> <stationid>MD180702</stationid> <stationid>MD180703</stationid> <stationid>MD180703</stationid> <stationid>MD180703</stationid> <stationid>MD180704</stationid> <stationid>MD180704</stationid> <stationid>MD180704</stationid> <stationid>MD180705</stationid> <stationid>MD180705</stationid> <stationid>MD180705</stationid> <stationid>MD180705</stationid> " & _
                                      "<stationid>MD180706</stationid> <stationid>MD180706</stationid> <stationid>MD180706</stationid> <stationid>MD180795</stationid> <stationid>MD180800</stationid> <stationid>MD180800</stationid> <stationid>MD180800</stationid> <stationid>MD181125</stationid> <stationid>MD181135</stationid> <stationid>MD181135</stationid> <stationid>MD181170</stationid> <stationid>MD181278</stationid> <stationid>MD181685</stationid> <stationid>MD181685</stationid> <stationid>MD181685</stationid> <stationid>MD181710</stationid> <stationid>MD181710</stationid> <stationid>MD181710</stationid> <stationid>MD181862</stationid> <stationid>MD181862</stationid> <stationid>MD181862</stationid> <stationid>MD181995</stationid> <stationid>MD181995</stationid> <stationid>MD181995</stationid> <stationid>MD181995</stationid> <stationid>MD182325</stationid> <stationid>MD182325</stationid> <stationid>MD182325</stationid> <stationid>MD182585</stationid> <stationid>MD182585</stationid> <stationid>MD182585</stationid> <stationid>MD182660</stationid> " & _
                                      "<stationid>MD182660</stationid> <stationid>MD182660</stationid> <stationid>MD183230</stationid> <stationid>MD183230</stationid> <stationid>MD183230</stationid> <stationid>MD183645</stationid> <stationid>MD183675</stationid> <stationid>MD183675</stationid> <stationid>MD183675</stationid> <stationid>MD183860</stationid> <stationid>MD183860</stationid> <stationid>MD183860</stationid> <stationid>MD185080</stationid> <stationid>MD185080</stationid> <stationid>MD185080</stationid> <stationid>MD185111</stationid> <stationid>MD185111</stationid> <stationid>MD185111</stationid> <stationid>MD185201</stationid> <stationid>MD185201</stationid> <stationid>MD185201</stationid> <stationid>MD185718</stationid> <stationid>MD185718</stationid> <stationid>MD185718</stationid> <stationid>MD185718</stationid> <stationid>MD185865</stationid> <stationid>MD185865</stationid> <stationid>MD185865</stationid> <stationid>MD185916</stationid> <stationid>MD185916</stationid> <stationid>MD185916</stationid> <stationid>MD186350</stationid> " & _
                                      "<stationid>MD186350</stationid> <stationid>MD186350</stationid> <stationid>MD186770</stationid> <stationid>MD186770</stationid> <stationid>MD186770</stationid> <stationid>MD186915</stationid> <stationid>MD186915</stationid> <stationid>MD186915</stationid> <stationid>MD187325</stationid> <stationid>MD187325</stationid> <stationid>MD187325</stationid> <stationid>MD187615</stationid> <stationid>MD187615</stationid> <stationid>MD187615</stationid> <stationid>MD187705</stationid> <stationid>MD187705</stationid> <stationid>MD187705</stationid> <stationid>MD188656</stationid> <stationid>MD188656</stationid> <stationid>MD188656</stationid> <stationid>MD188725</stationid> <stationid>MD188725</stationid> <stationid>MD188725</stationid> <stationid>MD189035</stationid> <stationid>MD189035</stationid> <stationid>MD189035</stationid> <stationid>MD189070</stationid> <stationid>MD189070</stationid> <stationid>MD189070</stationid> <stationid>MD189187</stationid> <stationid>MD189187</stationid> <stationid>MD189187</stationid> " & _
                                      "<stationid>MD189195</stationid> <stationid>MD189195</stationid> <stationid>MD189195</stationid> <stationid>MD189290</stationid> <stationid>MD189290</stationid> <stationid>MD189290</stationid> <stationid>MD189290</stationid> <stationid>MD189314</stationid> <stationid>MD189314</stationid> <stationid>MD189314</stationid> <stationid>MD189502</stationid> <stationid>MD189502</stationid> <stationid>MD189502</stationid> <stationid>MD189750</stationid> <stationid>MD189750</stationid> <stationid>MD189750</stationid> <stationid>MD724040</stationid> <stationid>MD724040</stationid> <stationid>MD724040</stationid> <stationid>MD724040</stationid> <stationid>MD724040</stationid> <stationid>MD724040</stationid> <stationid>MD745940</stationid> <stationid>MD745940</stationid> <stationid>MD745940</stationid> <stationid>MD745940</stationid> <stationid>MD745940</stationid> <stationid>MD745940</stationid> <stationid>MD994400</stationid> <stationid>MD994400</stationid> <stationid>MD994400</stationid> <stationid>VA440090</stationid> " & _
                                      "<stationid>VA440090</stationid> <stationid>VA440090</stationid> <stationid>VA440097</stationid> <stationid>VA440097</stationid> <stationid>VA440097</stationid> <stationid>VA441729</stationid> <stationid>VA442195</stationid> <stationid>VA442195</stationid> <stationid>VA442195</stationid> <stationid>VA442809</stationid> <stationid>VA442809</stationid> <stationid>VA442809</stationid> <stationid>VA442922</stationid> <stationid>VA442922</stationid> <stationid>VA442922</stationid> <stationid>VA443635</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448906</stationid> <stationid>VA448938</stationid> <stationid>VA448938</stationid> <stationid>VA448938</stationid> <clip>False</clip> <merge>False</merge> <joinattributes>true</joinattributes> </arguments> </function>  "
        Dim lResultMetData As String = lDownloadManager.Execute(lQueryMetData)
        If lResultMetData Is Nothing OrElse lResultMetData.Length = 0 Then
            'Nothing to report, no success or error
        ElseIf lResultMetData.StartsWith("<success>") Then
            BASINS.ProcessDownloadResults(lResultMetData)
        Else
            Logger.Msg(atcUtility.ReadableFromXML(lResultMetData), "MetDataDownload Result")
        End If
        Debug.Print("ProjectSaved:" & pMapWin.Project.Save(pMapWin.Project.FileName))

        Return True
    End Function

    Private Function Exercise2() As Boolean
        Logger.Dbg("NotYetImplemented")
        Return False
    End Function

End Module
