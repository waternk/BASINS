Imports MapWinUtility
Imports System.Text
Imports atcUtility

Public Class WASPProject
    Public Segments As Segments
    Public InputTimeseriesCollection As WASPTimeseriesCollection

    Public Name As String = ""
    Public WNFFileName As String = ""

    Public Sub New()
        Name = ""
        WNFFileName = ""
        Segments = New Segments
        Segments.WASPProject = Me
        InputTimeseriesCollection = New WASPTimeseriesCollection
    End Sub

    Public Function Save(ByVal aFileName As String) As Boolean

        'set file names
        WNFFileName = aFileName
        Dim lSegmentFileName As String = FilenameSetExt(WNFFileName, "seg")
        Dim lDirectoryFileName As String = FilenameSetExt(WNFFileName, "tim")

        'write WASP network file first
        Dim lSW As New IO.StreamWriter(aFileName)
        lSW.WriteLine(lSegmentFileName)
        lSW.WriteLine(lDirectoryFileName)
        lSW.Close()

        'write segments file
        lSW = New IO.StreamWriter(lSegmentFileName)
        lSW.WriteLine(Segments.ToString)
        lSW.Close()

        'write timeseries directory file
        lSW = New IO.StreamWriter(lDirectoryFileName)
        lSW.WriteLine(Segments.TimeseriesDirectoryToString)
        lSW.Close()

        'write timeseries files
        Me.InputTimeseriesCollection.TimeSeriesToFile(lDirectoryFileName)

        Return True
    End Function

    Public Sub Run(ByVal aInputFileName As String)
        If IO.File.Exists(aInputFileName) Then
            Dim lWASPDir As String = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\USEPA\WASP\7.0", "DatabaseDir", "") & "..\wasp.exe"
            Dim lWASPexe As String = atcUtility.FindFile("Please locate the EPA WASP Executable", lWASPDir)
            If IO.File.Exists(lWASPexe) Then
                LaunchProgram(lWASPexe, IO.Path.GetDirectoryName(aInputFileName), aInputFileName, False)
                Logger.Dbg("WASP launched with input " & aInputFileName)
            Else
                Logger.Msg("Cannot find the EPA WASP Executable", MsgBoxStyle.Critical, "BASINS WASP Problem")
            End If
        Else
            Logger.Msg("Cannot find WASP Input File " & aInputFileName)
        End If
    End Sub
End Class
