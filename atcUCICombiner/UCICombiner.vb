Imports MapWinUtility
Imports atcUtility
Imports atcData
Imports atcWDM
Imports System.Collections.Specialized

Public Module UCICombiner
    Private pBaseDir As String = "D:\cbp_working\"
    Private pWorkingDir As String = pBaseDir & "output\"  '= "subset\"
    Private pOutputDir As String = pWorkingDir & "combined\"
    Private pWatershed As String = "Monocacy"
    Private pLandUseTableDir As String = pBaseDir & "model\p512\pp\catalog\iovars\"

    Public Sub Main()
        ChDriveDir(pWorkingDir)
        Logger.StartToFile(pOutputDir & "uciCombiner.log")

        Dim lMsg As New atcUCI.HspfMsg
        lMsg.Open("hspfmsg.mdb")

        'get names of all ucis in dir
        Dim lUcis As Collection = HSPFUciNames(pWorkingDir)
        Logger.Dbg("UCICount " & lUcis.Count)

        'create a new uci to be the combined uci
        Dim lCombinedUci As New atcUCI.HspfUci

        ''for testing uci save
        'lWorkingDir = "C:\cbp_working\output\CBP716Copy\"
        'ChDir(lWorkingDir)
        'lCombinedUci.FastReadUciForStarter(lMsg, "base.uci")
        'ChDir(lWorkingDir & "temp")
        'lCombinedUci.Save()

        'read first uci
        lCombinedUci.FastReadUciForStarter(lMsg, lUcis(1))
        lCombinedUci.MetSeg2Source()
        lCombinedUci.Point2Source()

        'make this the combined uci
        lCombinedUci.Name = "base.uci"
        lCombinedUci.GlobalBlock.RunInf.Value = "Base UCI for " & pWatershed

        Dim lMetSegCounter As Integer = 100
        Dim lLandUseCounter As Integer = 1
        Dim lOrigId As Integer
        Dim lTotalMetSegCount As Integer = 7

        'change this operation number
        Dim lOper As atcUCI.HspfOperation
        lOper = lCombinedUci.OpnBlks("PERLND").ids(1)
        lOrigId = lOper.Id
        lOper.Id = 101
        'remove all the targets from this perlnd
        Dim lConn As atcUCI.HspfConnection
        For Each lConn In lOper.Targets
            lOper.Targets.Remove(1)
        Next lConn
        'renumber data sets to reflect met seg number
        For Each lConn In lOper.Sources
            lConn.Source.VolId = lConn.Source.VolId + lMetSegCounter
        Next lConn

        'change operation number in all special actions
        Dim lRecord As atcUCI.HspfSpecialRecord
        Dim lUvQuanIndex As Integer
        RenumberOperationInSpecialActions(lCombinedUci, "PERLND", lOrigId, lOper.Id)
        Dim lRecordIndex As Integer = 1
        Do While lRecordIndex < lCombinedUci.SpecialActionBlk.Records.Count
            lRecord = lCombinedUci.SpecialActionBlk.Records(lRecordIndex)
            If InStr(lRecord.Text, "+=         0.") = 58 Then
                lCombinedUci.SpecialActionBlk.Records.Remove(lRecordIndex)
            ElseIf lRecord.SpecType = atcUCI.HspfData.HspfSpecialRecordType.hUserDefineQuan And _
                Mid(lRecord.Text, 1, 16) = "  UVQUAN prec   " Then
                'need one of these for each met segment
                lRecord.Text = "  UVQUAN prec1  " & Mid(lRecord.Text, 17)
                lUvQuanIndex = lRecordIndex
            Else
                If Mid(lRecord.Text, 1, 9) = "IF (prec " Then
                    'update to reflect met segment number
                    lRecord.Text = "IF (prec1 " & Mid(lRecord.Text, 10)
                End If
                lRecordIndex += 1
            End If
        Loop

        'save names of met and precip wdms for each met seg
        Dim lMetWDMNames As New Collection
        Dim lPrecWDMNames As New Collection
        lMetWDMNames.Add(lCombinedUci.FilesBlock.Value(1).Name)
        lPrecWDMNames.Add(lCombinedUci.FilesBlock.Value(2).Name)
        'save names of each pt src and output wdm for each rchres
        Dim lPtSrcWDMNames As New Collection
        Dim lOutputWDMNames As New Collection

        'update files block
        UpdateFilesBlock(lCombinedUci)

        Logger.Dbg("CombinedUCI build based on " & lUcis(1))

        'now start looping through the rest of the ucis
        For lUciIndex As Integer = 2 To lUcis.Count
            'read each uci 
            Dim lUci As New atcUCI.HspfUci
            lUci.FastReadUciForStarter(lMsg, lUcis(lUciIndex))
            lUci.MetSeg2Source()
            lUci.Point2Source()

            'add operations of second uci into first uci
            Dim lNewOperId As Integer
            For Each lOper In lUci.OpnSeqBlock.Opns
                If lOper.Name = "PERLND" Or lOper.Name = "IMPLND" Then
                    lMetSegCounter = lMetSegCounter + 100
                    If lMetSegCounter > lTotalMetSegCount * 100 Then
                        'this will be a new land use
                        lLandUseCounter = lLandUseCounter + 1
                        lMetSegCounter = 100
                    End If
                    'save the names of the wdm files associated with each met segment
                    lMetWDMNames.Add(lUci.FilesBlock.Value(1).Name)
                    lPrecWDMNames.Add(lUci.FilesBlock.Value(2).Name)
                    lNewOperId = lMetSegCounter + lLandUseCounter
                Else
                    lNewOperId = 1
                End If
                'make sure this is a unique number
                Do While Not lCombinedUci.OpnBlks(lOper.Name).operfromid(lNewOperId) Is Nothing
                    lNewOperId = lNewOperId + 1
                Loop

                'add this operation
                Dim lOpn As New atcUCI.HspfOperation
                lOpn = lOper
                lOpn.Name = lOper.Name
                lOrigId = lOper.Id
                lOpn.Id = lNewOperId
                lOpn.Uci = lCombinedUci
                lCombinedUci.OpnBlks(lOper.Name).Ids.Add(lOpn, "K" & lOpn.Id)
                lOpn.OpnBlk = lCombinedUci.OpnBlks(lOper.Name)
                lCombinedUci.OpnSeqBlock.Add(lOper)

                'remove the comments so we don't get repeated headers
                For Each lTable As atcUCI.HspfTable In lOpn.Tables
                    lTable.Comment = ""
                Next lTable

                If lOper.Name = "IMPLND" Then
                    'make adjustment to GEN-INFO table where cbp fields are off
                    lOper.Tables("GEN-INFO").parms(4).value = lOper.Tables("GEN-INFO").parms(5).value
                    lOper.Tables("GEN-INFO").parms(5).value = lOper.Tables("GEN-INFO").parms(6).value
                    lOper.Tables("GEN-INFO").parms(6).value = lOper.Tables("GEN-INFO").parms(7).value
                End If

                If lOper.Name = "RCHRES" Then
                    'update ftable number
                    lOper.FTable.Id = lNewOperId
                    lOper.Tables("HYDR-PARM2").parmvalue("FTBUCI") = lNewOperId
                    'make note of which met segment this uci uses
                    For i As Integer = 1 To lPrecWDMNames.Count
                        If lPrecWDMNames(i) = lUci.FilesBlock.Value(2).Name Then
                            lMetSegCounter = i * 100
                            Exit For
                        End If
                    Next i
                    'store name of the pt src wdm this uci uses (needs to be modified slightly)
                    lPtSrcWDMNames.Add(Mid(lUci.FilesBlock.Value(3).Name, 1, 17) & "609" & Mid(lUci.FilesBlock.Value(3).Name, 21))
                    'store name of the output wdm this uci uses
                    lOutputWDMNames.Add(lUci.FilesBlock.Value(4).Name)
                End If

                If lOper.Name = "PERLND" Or lOper.Name = "IMPLND" Then
                    'remove all the targets from perlnds and implnds
                    '(we can pass internally without writing to wdm)
                    For Each lConn In lOpn.Targets
                        lOpn.Targets.Remove(1)
                    Next lConn
                    'renumber data sets to reflect met seg number
                    For Each lConn In lOpn.Sources
                        If lConn.Target.VolName = lOper.Name Then
                            'renumber the dsn in the ext sources block
                            lConn.Source.VolId = lConn.Source.VolId + lMetSegCounter
                        End If
                    Next lConn
                ElseIf lOper.Name = "RCHRES" Then
                    'remove all the sources coming from upstream through wdms
                    '(we can pass internally without reading from wdm)
                    Dim i As Integer = 1
                    For Each lConn In lOper.Sources
                        If lConn.Source.VolName = "WDM4" Then
                            lOper.Sources.Remove(i)
                        Else
                            i = i + 1
                        End If
                    Next lConn
                    'renumber data sets to reflect met seg number
                    For Each lConn In lOpn.Sources
                        If lConn.Target.VolName = lOper.Name Then
                            'renumber the dsn in the ext sources block
                            lConn.Source.VolId = lConn.Source.VolId + lMetSegCounter
                        End If
                    Next lConn
                    'reset the connection operation numbers
                    For Each lConn In lOper.Targets
                        If lConn.Source.VolName = "RCHRES" Then
                            lConn.Source.Opn.Id = lNewOperId
                            lConn.Source.VolId = lNewOperId
                            If lConn.Target.VolName = "WDM4" Then
                                lConn.Target.VolId = lConn.Target.VolId + (100 * (lNewOperId - 1))
                            End If
                        End If
                    Next lConn
                    For Each lConn In lOper.Sources
                        If lConn.Target.VolName = "RCHRES" Then
                            If Not lConn.Target.Opn Is Nothing Then
                                lConn.Target.Opn.Id = lNewOperId
                            End If
                            lConn.Target.VolId = lNewOperId
                        End If
                    Next lConn
                End If

                RenumberOperationInSpecialActions(lUci, lOper.Name, lOrigId, lOper.Id)
                'now add the special actions records to the uci
                For Each lRecord In lUci.SpecialActionBlk.Records
                    If lRecord.SpecType = atcUCI.HspfData.HspfSpecialRecordType.hAction Or _
                       lRecord.SpecType = atcUCI.HspfData.HspfSpecialRecordType.hCondition Or _
                       lRecord.SpecType = atcUCI.HspfData.HspfSpecialRecordType.hUserDefineName Then
                        If Mid(lRecord.Text, 1, 9) = "IF (prec " Then
                            'update to reflect met segment number
                            lRecord.Text = "IF (prec" & Mid(CStr(lOper.Id), 1, 1) & " " & Mid(lRecord.Text, 10)
                        End If
                        If InStr(lRecord.Text, "+=         0.") <> 58 Then
                            lCombinedUci.SpecialActionBlk.Records.Add(lRecord)
                        End If
                    End If
                Next
            Next lOper

            lUci = Nothing
            Logger.Dbg("Added " & lUcis(lUciIndex))
        Next lUciIndex

        'build connections
        AddSchematicBlock(lCombinedUci)
        Logger.Dbg("ConnectionsBuilt")

        'build mass links
        AddMassLinks(lCombinedUci)
        Logger.Dbg("MassLinksBuilt")

        'remove duplicate mass links
        FilterMassLinks(lCombinedUci)
        Logger.Dbg("MassLinkDuplicatesRemoved")

        'convert back to met segs to look nice in the save
        lCombinedUci.Source2MetSeg()
        Logger.Dbg("MetSegmentsPolished")

        'add uvquans for each met segment
        lRecord = lCombinedUci.SpecialActionBlk.Records(lUvQuanIndex)
        For i As Integer = 2 To lTotalMetSegCount
            Dim lNewRecord As New atcUCI.HspfSpecialRecord
            lNewRecord.SpecType = lRecord.SpecType
            lNewRecord.Text = "  UVQUAN prec" & CStr(i) & "  " & Mid(lRecord.Text, 17, 7) & CStr(i) & Mid(lRecord.Text, 25)
            lCombinedUci.SpecialActionBlk.Records.Add(lNewRecord, , , lUvQuanIndex)
            lUvQuanIndex = lUvQuanIndex + 1
        Next i
        Logger.Dbg("UvquansAdded")

        'combine the WDM files used by the combined UCI
        ChDir(pOutputDir)
        BuildCombinedWDMs(lCombinedUci, lMetWDMNames, lPrecWDMNames, lPtSrcWDMNames, lOutputWDMNames)
        Logger.Dbg("CombinedWdmsBuilt")

        'write the combined uci 
        lCombinedUci.Save()
        Logger.Dbg("CombinedUCI Saved")

    End Sub

    Private Function HSPFUciNames(ByVal aWorkingDir As String) As Collection
        Dim lPerlndUcis As New Collection
        Dim lImplndUcis As New Collection
        Dim lRchresUcis As New Collection
        Dim lUciFullNames As New NameValueCollection
        Dim lString As String
        Dim lUciName As String
        Dim i As Integer
        Dim lUcis As New Collection

        AddFilesInDir(lUciFullNames, aWorkingDir, False, "*.uci")
        Logger.Dbg("Processing " & lUciFullNames.Count & " ucis")

        'we could open each uci and look to see what operation it contains,
        'but we know based on the naming convention
        For Each lString In lUciFullNames
            lUciName = FilenameNoPath(lString)
            If Mid(lUciName, 1, 3) = "afo" Or _
               Mid(lUciName, 1, 3) = "imh" Or _
               Mid(lUciName, 1, 3) = "iml" Then
                lImplndUcis.Add(lUciName)
            ElseIf Mid(lUciName, 1, 3) = "riv" Then
                lRchresUcis.Add(lUciName)
            Else
                lPerlndUcis.Add(lUciName)
            End If
        Next lString
        'put them in order
        For i = 1 To lPerlndUcis.Count
            lUcis.Add(lPerlndUcis(i))
        Next i
        For i = 1 To lImplndUcis.Count
            lUcis.Add(lImplndUcis(i))
        Next i
        For i = 1 To lRchresUcis.Count
            lUcis.Add(lRchresUcis(i))
        Next i

        HSPFUciNames = lUcis

    End Function

    Private Function UpdateFilesBlock(ByVal aUci As atcUCI.HspfUci) As Boolean
        Dim lHspfFile As New atcUCI.HspfData.HspfFile
        With aUci.FilesBlock
            lHspfFile.Name = "base.wdm"
            lHspfFile.Typ = "WDM1"
            lHspfFile.Unit = "21"
            .Value(1) = lHspfFile
            lHspfFile.Name = "ptsrc.wdm"
            lHspfFile.Typ = "WDM3"
            lHspfFile.Unit = "23"
            .Value(2) = lHspfFile
            lHspfFile.Name = "output.wdm"
            lHspfFile.Typ = "WDM4"
            lHspfFile.Unit = "24"
            .Value(3) = lHspfFile
            lHspfFile.Name = "base.ech"
            lHspfFile.Typ = "MESSU"
            lHspfFile.Unit = "25"
            .Value(4) = lHspfFile
            lHspfFile.Name = "base.out"
            lHspfFile.Typ = ""
            lHspfFile.Unit = "26"
            .Value(5) = lHspfFile
        End With
    End Function

    Private Function RenumberOperationInSpecialActions(ByVal aUci As atcUCI.HspfUci, ByVal aOperName As String, ByVal aOrigId As Integer, ByVal aNewId As Integer) As Boolean
        Dim lRecord As atcUCI.HspfSpecialRecord
        Dim i As Integer

        For Each lRecord In aUci.SpecialActionBlk.Records
            i = InStr(lRecord.Text, aOperName & "  " & aOrigId)
            If i > 0 Then
                Mid(lRecord.Text, i) = aOperName & aNewId
            End If
            i = InStr(lRecord.Text, aOperName & "   " & aOrigId)
            If i > 0 Then
                Mid(lRecord.Text, i) = aOperName & " " & aNewId
            End If
        Next
    End Function

    Private Function AddSchematicBlock(ByVal aCombinedUci As atcUCI.HspfUci) As Boolean
        'build connections 
        Dim i As Integer

        Dim lAreaTable As New atcTableDBF
        lAreaTable.OpenFile("C:\cbp_working\model\p512\pp\data\land_use\land_use_base10_2002.dbf")
        'look through each rchres
        Dim lRchOper As atcUCI.HspfOperation
        Dim lLandOper As atcUCI.HspfOperation
        Dim lConn As atcUCI.HspfConnection
        Dim lLandUse As String
        Dim lOperType As String
        Dim lFieldNum(2) As Integer
        Dim lTableOper(2) As String
        Dim lFieldVal(2) As String
        Dim lArea As Single
        lFieldNum(1) = 1
        lFieldNum(2) = 2
        lTableOper(1) = "="
        lTableOper(2) = "="
        Dim lRchIDs As New Collection
        Dim lDownRchIDs As New Collection
        For Each lRchOper In aCombinedUci.OpnBlks("RCHRES").ids
            lFieldVal(1) = lRchOper.Tables("GEN-INFO").parms("RCHID").value
            For i = 1 To 2
                If i = 1 Then
                    lOperType = "PERLND"
                Else
                    lOperType = "IMPLND"
                End If
                For Each lLandOper In aCombinedUci.OpnBlks(lOperType).ids
                    lFieldVal(2) = Mid(lLandOper.Tables("GEN-INFO").parms("LSID").value, 1, 6)
                    lLandUse = Trim(Mid(lLandOper.Tables("GEN-INFO").parms("LSID").value, 7))
                    If lAreaTable.FindMatch(lFieldNum, lTableOper, lFieldVal) Then
                        'found this land series contributing to this reach
                        lArea = lAreaTable.Value(lAreaTable.FieldNumber(UCase(lLandUse)))
                        If lArea > 0 Then
                            lConn = New atcUCI.HspfConnection
                            lConn.Uci = aCombinedUci
                            lConn.Typ = 3
                            lConn.Source.VolName = lLandOper.Name
                            lConn.Source.VolId = lLandOper.Id
                            lConn.MFact = lArea
                            lConn.Target.VolName = "RCHRES"
                            lConn.Target.VolId = lRchOper.Id
                            lConn.MassLink = Mid(CStr(lLandOper.Id), 2)
                            aCombinedUci.Connections.Add(lConn)
                            lLandOper.Targets.Add(lConn)
                            lRchOper.Sources.Add(lConn)
                        End If
                    End If
                Next lLandOper
            Next i
            'also get reach to reach connections out of the reach id
            lRchIDs.Add(Mid(lFieldVal(1), 5, 4))
            lDownRchIDs.Add(Mid(lFieldVal(1), 10, 4))
        Next lRchOper
        'add reach to reach connections
        Dim j As Integer
        For i = 1 To lRchIDs.Count
            For j = 1 To lRchIDs.Count
                If lRchIDs(j) = lDownRchIDs(i) Then
                    lConn = New atcUCI.HspfConnection
                    lConn.Uci = aCombinedUci
                    lConn.Typ = 3
                    lConn.Source.VolName = "RCHRES"
                    lConn.Source.VolId = i
                    lConn.MFact = 1.0#
                    lConn.Target.VolName = "RCHRES"
                    lConn.Target.VolId = j
                    lConn.MassLink = 99
                    aCombinedUci.Connections.Add(lConn)
                    aCombinedUci.OpnBlks("RCHRES").operfromid(i).targets.add(lConn)
                    aCombinedUci.OpnBlks("RCHRES").operfromid(j).sources.add(lConn)
                End If
            Next j
        Next i
    End Function

    Private Function AddMassLinks(ByVal aCombinedUci As atcUCI.HspfUci) As Boolean
        'build mass links 
        Dim i As Integer
        Dim lMassLink As atcUCI.HspfMassLink
        Dim lLandType As String
        Dim lLandOper As atcUCI.HspfOperation
        Dim lLandUse As String

        aCombinedUci.MassLinks.Remove(1)
        Dim lMultTable As New atcTableFixed
        lMultTable.NumFields = 3
        lMultTable.FieldStart(1) = 9     'riv id
        lMultTable.FieldLength(1) = 3
        lMultTable.FieldStart(2) = 23    'land id
        lMultTable.FieldLength(2) = 3
        lMultTable.FieldStart(3) = 36    'mfact
        lMultTable.FieldLength(3) = 8
        lMultTable.OpenFile("C:\cbp_working\model\p512\pp\catalog\iovars\land_to_river")
        Dim lRchresTable As New atcTableFixed
        lRchresTable.NumFields = 5
        lRchresTable.FieldStart(1) = 7   'riv id
        lRchresTable.FieldLength(1) = 3
        lRchresTable.FieldStart(2) = 14  'group
        lRchresTable.FieldLength(2) = 6
        lRchresTable.FieldStart(3) = 21  'member 
        lRchresTable.FieldLength(3) = 6
        lRchresTable.FieldStart(4) = 28  'mems1
        lRchresTable.FieldLength(4) = 1
        lRchresTable.FieldStart(5) = 30  'mems2
        lRchresTable.FieldLength(5) = 1
        lRchresTable.OpenFile("C:\cbp_working\model\p512\pp\catalog\iovars\rchres_in")
        Dim lLandTable As New atcTableFixed
        lLandTable.NumFields = 6
        lLandTable.FieldStart(1) = 1   'land id
        lLandTable.FieldLength(1) = 3
        lLandTable.FieldStart(2) = 7   'group
        lLandTable.FieldLength(2) = 6
        lLandTable.FieldStart(3) = 14  'member 
        lLandTable.FieldLength(3) = 6
        lLandTable.FieldStart(4) = 21  'mems1
        lLandTable.FieldLength(4) = 1
        lLandTable.FieldStart(5) = 23  'mems2
        lLandTable.FieldLength(5) = 1
        lLandTable.FieldStart(6) = 68  'land use names
        lLandTable.FieldLength(6) = 88

        'now loop through each perlnd/implnd record making mass links
        For i = 1 To 2
            Dim lResult As Boolean
            If i = 1 Then
                lLandType = "PERLND"
                lResult = lLandTable.OpenFile(pLandUseTableDir & "perlnd")
            Else
                lLandType = "IMPLND"
                lResult = lLandTable.OpenFile(pLandUseTableDir & "implnd")
            End If
            If lResult Then
                For Each lLandOper In aCombinedUci.OpnBlks(lLandType).ids
                    If lLandOper.Id < 200 Then
                        'only need to do for the first segment

                        lLandUse = Trim(Mid(lLandOper.Tables("GEN-INFO").parms("LSID").value, 7))

                        lLandTable.MoveFirst()
                        Do Until lLandTable.atEOF
                            If IsNumeric(lLandTable.Value(1)) And InStr(lLandTable.Value(6), lLandUse) > 0 Then
                                'look for this land id in mfacts table
                                lMultTable.MoveFirst()
                                Do Until lMultTable.atEOF
                                    If lLandTable.Value(1) = lMultTable.Value(2) Then
                                        'look for this riv id in rchres table
                                        lRchresTable.MoveFirst()
                                        Do Until lRchresTable.atEOF
                                            If lMultTable.Value(1) = lRchresTable.Value(1) Then
                                                'this is a hit, add it as a mass link
                                                lMassLink = New atcUCI.HspfMassLink
                                                lMassLink.Uci = aCombinedUci
                                                lMassLink.MassLinkID = Mid(CStr(lLandOper.Id), 2)
                                                lMassLink.Source.VolName = lLandType
                                                lMassLink.Source.VolId = 0
                                                lMassLink.Source.Group = lLandTable.Value(2)
                                                lMassLink.Source.Member = lLandTable.Value(3)
                                                If IsNumeric(lLandTable.Value(4)) Then
                                                    lMassLink.Source.MemSub1 = lLandTable.Value(4)
                                                End If
                                                If IsNumeric(lLandTable.Value(5)) Then
                                                    lMassLink.Source.MemSub2 = lLandTable.Value(5)
                                                End If
                                                If IsNumeric(lMultTable.Value(3)) Then
                                                    lMassLink.MFact = lMultTable.Value(3)
                                                End If
                                                lMassLink.Tran = ""
                                                lMassLink.Target.VolName = "RCHRES"
                                                lMassLink.Target.VolId = 0
                                                lMassLink.Target.Group = lRchresTable.Value(2)
                                                lMassLink.Target.Member = lRchresTable.Value(3)
                                                If IsNumeric(lRchresTable.Value(4)) Then
                                                    lMassLink.Target.MemSub1 = lRchresTable.Value(4)
                                                End If
                                                If IsNumeric(lRchresTable.Value(5)) Then
                                                    lMassLink.Target.MemSub2 = lRchresTable.Value(5)
                                                End If
                                                aCombinedUci.MassLinks.Add(lMassLink)
                                            End If
                                            lRchresTable.MoveNext()
                                        Loop
                                    End If
                                    lMultTable.MoveNext()
                                Loop
                            End If
                            lLandTable.MoveNext()
                        Loop
                    End If
                Next lLandOper
            Else
                Throw New Exception("Failed to Open LandUseTable for " & lLandType & _
                                    " in '" & pLandUseTableDir & "'")
            End If
        Next i

        'also add the rchres to rchres mass-link
        lMassLink = New atcUCI.HspfMassLink
        lMassLink.Uci = aCombinedUci
        lMassLink.MassLinkID = 99
        lMassLink.Source.VolName = "RCHRES"
        lMassLink.Source.VolId = 0
        lMassLink.Source.Group = "OFLOW"
        lMassLink.Source.Member = ""
        lMassLink.MFact = 1.0#
        lMassLink.Tran = ""
        lMassLink.Target.VolName = "RCHRES"
        lMassLink.Target.VolId = 0
        lMassLink.Target.Group = "INFLOW"
        lMassLink.Target.Member = ""
        aCombinedUci.MassLinks.Add(lMassLink)
    End Function

    Private Function FilterMassLinks(ByVal aCombinedUci As atcUCI.HspfUci) As Boolean
        'remove identical mass links 
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim lMassLink As atcUCI.HspfMassLink
        Dim mlno(-1) As Integer
        Dim mlcnt As Integer = 0
        Dim found As Boolean
        Dim lIdentical As Boolean
        Dim lML1 As atcUCI.HspfMassLink
        Dim lML2 As atcUCI.HspfMassLink
        Dim lConn As atcUCI.HspfConnection

        'develop list of mass link ids
        For j = 1 To aCombinedUci.MassLinks.Count()
            lMassLink = aCombinedUci.MassLinks(j)
            found = False
            For k = 0 To mlcnt - 1
                If lMassLink.MassLinkID = mlno(k) Then
                    found = True
                End If
            Next k
            If found = False Then
                mlcnt = mlcnt + 1
                ReDim Preserve mlno(mlcnt)
                mlno(mlcnt - 1) = lMassLink.MassLinkID
            End If
        Next j

        For i = 1 To mlcnt - 1
            'see if this ml is like any previous
            Dim lUmls As New Collection
            For Each lMassLink In aCombinedUci.MassLinks
                If lMassLink.MassLinkID = mlno(i) Then
                    lUmls.Add(lMassLink)
                End If
            Next lMassLink
            For j = 0 To i - 1
                Dim lLmls As New Collection
                For Each lMassLink In aCombinedUci.MassLinks
                    If lMassLink.MassLinkID = mlno(j) Then
                        lLmls.Add(lMassLink)
                    End If
                Next lMassLink
                'now compare
                lIdentical = True
                If lLmls.Count <> lUmls.Count Then
                    lIdentical = False
                Else
                    For k = 1 To lUmls.Count
                        lML1 = lLmls(k)
                        lML2 = lUmls(k)
                        If lML1.Source.VolName = lML2.Source.VolName And _
                           lML1.Source.Group = lML2.Source.Group And _
                           lML1.Source.Member = lML2.Source.Member And _
                           lML1.Source.MemSub1 = lML2.Source.MemSub1 And _
                           lML1.Source.MemSub2 = lML2.Source.MemSub2 And _
                           lML1.MFact = lML2.MFact And _
                           lML1.Target.VolName = lML2.Target.VolName And _
                           lML1.Target.Group = lML2.Target.Group And _
                           lML1.Target.Member = lML2.Target.Member And _
                           lML1.Target.MemSub1 = lML2.Target.MemSub1 And _
                           lML1.Target.MemSub2 = lML2.Target.MemSub2 Then
                        Else
                            lIdentical = False
                        End If
                    Next
                End If
                If lIdentical Then
                    'if identical, get rid of the second one
                    k = 1
                    For Each lMassLink In aCombinedUci.MassLinks
                        If lMassLink.MassLinkID = mlno(i) Then
                            aCombinedUci.MassLinks.Remove(k)
                        Else
                            k = k + 1
                        End If
                    Next lMassLink
                    'set schematic block records to first one
                    For Each lConn In aCombinedUci.Connections
                        If lConn.MassLink = mlno(i) Then
                            lConn.MassLink = mlno(j)
                        End If
                    Next
                End If
            Next j
        Next i
    End Function

    Private Function BuildCombinedWDMs(ByVal aCombinedUci As atcUCI.HspfUci, _
                                       ByVal aMetWDMNames As Collection, _
                                       ByVal aPrecWDMNames As Collection, _
                                       ByVal aPtSrcWDMNames As Collection, _
                                       ByVal aOutputWDMNames As Collection) As Boolean
        Dim lOper As atcUCI.HspfOperation
        Dim lConn As atcUCI.HspfConnection

        Try
            'build combined wdms for prec, met data 
            Dim lMetSeg As atcUCI.HspfMetSeg
            Dim lMetSegRec As atcUCI.HspfMetSegRecord
            Dim lWdmIndex As Integer
            Dim lOrigDsn As Integer
            For Each lMetSeg In aCombinedUci.MetSegs
                For Each lMetSegRec In lMetSeg.MetSegRecs
                    'the index is the second digit
                    If Not lMetSegRec Is Nothing Then
                        If Not lMetSegRec.Source.VolName Is Nothing Then
                            Logger.Dbg("CombineUcis:VolId:" & lMetSegRec.Source.VolId)
                            lWdmIndex = CInt(Mid(CStr(lMetSegRec.Source.VolId), 2, 1))
                            lOrigDsn = CInt(Mid(CStr(lMetSegRec.Source.VolId), 1, 1) & "0" & Mid(CStr(lMetSegRec.Source.VolId), 3, 2))
                            If lMetSegRec.Source.VolName = "WDM1" Then
                                CopyDataSet("wdm", aMetWDMNames(lWdmIndex), lOrigDsn, _
                                            "wdm", "base.wdm", lMetSegRec.Source.VolId)
                            ElseIf lMetSegRec.Source.VolName = "WDM2" Then
                                CopyDataSet("wdm", aPrecWDMNames(lWdmIndex), lOrigDsn, _
                                            "wdm", "base.wdm", lMetSegRec.Source.VolId)
                                lMetSegRec.Source.VolName = "WDM1"
                            End If
                            SetWDMAttribute("base.wdm", lMetSegRec.Source.VolId, "idscen", "OBSERVED")
                            SetWDMAttribute("base.wdm", lMetSegRec.Source.VolId, "idlocn", "SEG" & CStr(lWdmIndex))
                        End If
                    End If
                Next
            Next lMetSeg
            'build combined wdms for pt srcs and other input wdms
            For Each lOper In aCombinedUci.OpnSeqBlock.Opns
                For Each lConn In lOper.Sources
                    If Mid(lConn.Source.VolName, 1, 3) = "WDM" Then
                        lOrigDsn = CInt(Mid(CStr(lConn.Source.VolId), 1, 1) & "0" & Mid(CStr(lConn.Source.VolId), 3, 2))
                        lWdmIndex = CInt(Mid(CStr(lConn.Source.VolId), 2, 1))
                        If lConn.Source.VolName = "WDM3" Then
                            CopyDataSet("wdm", "..\" & aPtSrcWDMNames(lConn.Target.VolId), lOrigDsn, _
                                        "wdm", "ptsrc.wdm", lConn.Source.VolId)
                            SetWDMAttribute("ptsrc.wdm", lConn.Source.VolId, "idscen", "PT-OBS")
                        ElseIf lConn.Source.VolName = "WDM2" Then
                            CopyDataSet("wdm", aPrecWDMNames(lWdmIndex), lOrigDsn, _
                                        "wdm", "base.wdm", lConn.Source.VolId)
                            SetWDMAttribute("base.wdm", lConn.Source.VolId, "idscen", "OBSERVED")
                            lConn.Source.VolName = "WDM1"
                        End If
                    End If
                Next lConn
            Next lOper
            'build combined wdms for output wdms
            For Each lOper In aCombinedUci.OpnSeqBlock.Opns
                For Each lConn In lOper.Targets
                    If lConn.Target.VolName = "WDM4" Then
                        lOrigDsn = CInt("1" & Mid(CStr(lConn.Target.VolId), 2, 2))
                        CopyDataSet("wdm", "..\" & aOutputWDMNames(lConn.Source.VolId), lOrigDsn, _
                                    "wdm", "output.wdm", lConn.Target.VolId)
                        SetWDMAttribute("output.wdm", lConn.Target.VolId, "idscen", "BASE")
                        SetWDMAttribute("output.wdm", lConn.Target.VolId, "idlocn", "RIV" & CStr(lOper.Id))
                    End If
                Next lConn
            Next lOper
        Catch lEx As Exception
            Logger.Msg(lEx.ToString)
        End Try
    End Function
End Module
