'Copyright 2006-7 AQUA TERRA Consultants - Royalty-free use permitted under open source license
Option Strict Off
Option Explicit On

Imports System.Collections.ObjectModel
Imports System.Text
Imports MapWinUtility

Public Class HspfOpnBlks
    Inherits KeyedCollection(Of String, HspfOpnBlk)
    Protected Overrides Function GetKeyForItem(ByVal aHspfOpnBlk As HspfOpnBlk) As String
        Return aHspfOpnBlk.Name
    End Function
End Class

Public Class HspfOpnBlk
    Private pIds As HspfOperations
    Private pEdited As Boolean
    Private pTables As HspfTables
    Private pUci As HspfUci
    Public Comment As String = ""
    Public Name As String = ""

    Public Property Edited() As Boolean
        Get
            Return pEdited
        End Get
        Set(ByVal Value As Boolean)
            pEdited = Value
            If Value Then pUci.Edited = True
        End Set
    End Property

    Public ReadOnly Property Ids() As HspfOperations
        Get
            Return pIds
        End Get
    End Property

    Public Property Uci() As HspfUci
        Get
            Return pUci
        End Get
        Set(ByVal Value As HspfUci)
            pUci = Value
        End Set
    End Property

    ReadOnly Property Tables() As HspfTables
        Get 'of hspftables
            'make smarter if not found!!!
            'Set Tables = pIds(1).Tables
            Return pTables
        End Get
    End Property

    Public Function OperFromID(ByRef aId As Integer) As HspfOperation
        For Each lOperation As HspfOperation In Me.Ids
            If lOperation.Id = aId Then
                Return lOperation
            End If
        Next
        Return Nothing
    End Function

    Public Function NthOper(ByRef aNth As Integer) As HspfOperation
        Dim lOrder(pIds.Count) As Integer
        Dim lIds(pIds.Count) As Integer

        Dim lCnt As Integer = 0
        For Each lOperation As HspfOperation In Me.Ids
            lCnt += 1
            lIds(lCnt) = lOperation.Id
        Next

        SortIntegerArray(0, lCnt, lIds, lOrder)

        For Each lOperation As HspfOperation In Me.Ids
            If lOperation.Id = lIds(lOrder(aNth)) Then
                Return lOperation
            End If
        Next
        Return Nothing
    End Function

    Public Function Count() As Integer
        Return pIds.Count
    End Function

    Public Sub New()
        MyBase.New()
        pIds = New HspfOperations
        pTables = New HspfTables
    End Sub

    Public Function TableExists(ByRef aName As String) As Boolean
        Return pTables.Contains(aName)
    End Function

    Public Sub setTableValues(ByVal aBlockDef As HspfBlockDef)
        If pUci.FastFlag Then
            Comment = GetCommentBeforeBlock(Me.Name)
        End If
        ReadTables(aBlockDef)
    End Sub

    Private Sub ReadTables(ByRef aBlockDef As HspfBlockDef)
        'Logger.Dbg("Starting readTables at " & TimeOfDay)
        Dim lOperType As HspfData.HspfOperType = HspfOperNum(Me.Name)
        Dim lContinueFlag As Integer
        Dim lTableIndex As Integer = 0
        Do
            'return table names
            Dim lInit As Integer = 1
            Dim lKeyword As String = Nothing
            Dim lRetId As Integer = 0
            Dim lExistFlag As Integer
            Dim lOccurCount As Integer
            Dim lOccurNum As Integer
            Dim lSRec As Integer
            If pUci.FastFlag Then
                lKeyword = aBlockDef.TableDefs(lTableIndex).Name
                StartingRecordOfOperationTable(aBlockDef.Name, lKeyword, lSRec, lOccurCount)
                If lSRec > 0 Then
                    lExistFlag = 1 'does it exist, 1 if so
                Else
                    lExistFlag = 0
                End If
                lTableIndex += 1
                If lTableIndex < aBlockDef.TableDefs.Count Then
                    lContinueFlag = 1 'more tables to read flag, 1 if so
                Else
                    lContinueFlag = 0
                End If
                lRetId = (lOperType * 1000) + lTableIndex
            Else
                Call REM_GTNXKW((Me.Uci), lInit, CInt(lOperType + 120), lKeyword, lExistFlag, lContinueFlag, lRetId)
                lKeyword = AddChar2Keyword(lKeyword)
            End If
            lInit = 0
            If lExistFlag > 0 And lRetId <> 0 Then
                'check for multiple occurences
                If Not pUci.FastFlag Then
                    Call REM_GETOCR(Me.Uci, lRetId, lOccurCount)
                End If
                Dim lTable As HspfTable
                For lOccurNum = 1 To lOccurCount
                    Dim lTableComment As String = ""
                    If pUci.FastFlag Then
                        lTableComment = GetTableComment(lSRec, lKeyword, lOccurNum)
                    End If
                    Dim s() As String = {}
                    Dim c() As String = {}
                    Dim lScnt As Integer
                    Call GetTableRecordsFromUCI(lOperType + 120, aBlockDef.TableDefs.Item(lKeyword).SGRP, (aBlockDef.Name), lKeyword, lSRec, lOccurNum, lScnt, s, c)
                    For i As Integer = 1 To lScnt
                        'loop through each record in table
                        Dim lCombineOk As Boolean = True
                        Dim lOperFirst As Integer = CInt(s(i).Substring(0, 5).Trim)
                        Dim lOperLast As Integer = 0
                        Dim lOperLastS As String = ""
                        If s(i).Length >= 10 Then
                            lOperLastS = s(i).Substring(5, 5).Trim()
                        End If
                        If lOperLastS.Length > 0 Then
                            lOperLast = CInt(lOperLastS)
                        Else
                            lOperLast = lOperFirst
                            'check to see if this record could have been combined with the next record
                            If i > 1 Then
                                If compareTableString(1, 10, s(i), s(i - 1)) Then
                                    'if it could have but wasn't, assume the user wants it on its own line
                                    lCombineOk = False
                                End If
                            End If
                        End If
                        For Each lOperation As HspfOperation In pIds
                            If lOperFirst = lOperation.Id Or _
                              (lOperFirst <= lOperation.Id And lOperation.Id <= lOperLast) Then
                                lTable = New HspfTable
                                lTable.Opn = lOperation
                                lTable.Def = aBlockDef.TableDefs.Item(lKeyword)
                                lTable.initTable((s(i)))
                                lTable.Opn = lOperation
                                lTable.OccurCount = lOccurCount
                                lTable.OccurNum = lOccurNum
                                lTable.OccurIndex = 0
                                lTable.TableComment = lTableComment
                                lTable.CombineOK = lCombineOk
                                If Me.Name = "PERLND" And lTable.Def.Parent.Name = "PQUAL" Then
                                    'need to compute proper index
                                    If lOperation.TableExists("NQUALS") Then
                                        Dim lNQual As Integer = lOperation.Tables.Item("NQUALS").ParmValue("NQUAL")
                                        lTable.SetQualIndex(lOccurNum, lNQual)
                                    End If
                                End If
                                If Me.Name = "IMPLND" And lTable.Def.Parent.Name = "IQUAL" Then
                                    'need to compute proper index
                                    If lOperation.TableExists("NQUALS") Then
                                        Dim lNQual As Integer = lOperation.Tables.Item("NQUALS").ParmValue("NQUAL")
                                        lTable.SetQualIndex(lOccurNum, lNQual)
                                    End If
                                End If
                                If Me.Name = "RCHRES" And lTable.Def.Parent.Name = "GQUAL" Then
                                    'need to compute proper index
                                    If lOperation.TableExists("GQ-GENDATA") Then
                                        Dim lNGQual As Integer = lOperation.Tables.Item("GQ-GENDATA").ParmValue("NGQUAL")
                                        lTable.SetQualIndex(lOccurNum, lNGQual)
                                    End If
                                End If
                                If Len(c(i)) > 0 Then lTable.Comment = c(i)
                                If lOccurCount > 1 And lOccurNum > 1 Then
                                    lOperation.Tables.Add(lTable)
                                    If Not TableExists(lTable.Name & ":" & lOccurNum) Then
                                        pTables.Add(lTable)
                                    End If
                                Else
                                    Try
                                        lOperation.Tables.Add(lTable)
                                        If Not TableExists((lTable.Name)) Then
                                            pTables.Add(lTable)
                                        End If
                                    Catch
                                        Logger.Dbg("TableProblem:" & lTable.Name & ":" & lOperation.Id)
                                    End Try
                                End If
                                If lTable.Name = "GEN-INFO" Then
                                    lOperation.Description = lTable.Parms(0).Value
                                End If
                                If lTable.Name = "HYDR-PARM2" Then
                                    lOperation.FTable = New HspfFtable
                                    lOperation.FTable.Operation = lOperation
                                    lOperation.FTable.Id = lTable.ParmValue("FTBUCI")
                                End If
                            End If
                        Next lOperation
                    Next i
                    If lScnt = 0 Then
                        'still need to add the dummy table to this opnblk
                        lTable = New HspfTable
                        lTable.Opn = pIds.Item(1)
                        lTable.Def = aBlockDef.TableDefs.Item(lKeyword)
                        lTable.initTable((""))
                        lTable.OccurCount = lOccurCount
                        lTable.OccurNum = lOccurNum
                        lTable.OccurIndex = 0
                        lTable.TableComment = lTableComment
                        If lOccurCount > 1 And lOccurNum > 1 Then
                            If Not TableExists(lTable.Name & ":" & lOccurNum) Then
                                pTables.Add(lTable)
                            End If
                        Else
                            If Not TableExists((lTable.Name)) Then
                                pTables.Add(lTable)
                            End If
                        End If
                    End If
                Next lOccurNum
            End If
        Loop While lContinueFlag = 1
        'Logger.Dbg("Finishing readTables at " & TimeOfDay)
    End Sub

    Private Sub GetTableRecordsFromUCI(ByRef SCLU As Integer, ByRef SGRP As Integer, ByRef blockname As String, ByRef tablename As String, ByRef srec As Integer, ByRef thisoccur As Integer, ByRef scnt As Integer, ByRef s() As String, ByRef c() As String)
        Dim opf, retcod, uunits, tinit, retkey, sameoper, i As Integer
        Dim stemp As String = Nothing
        Dim pastHeader As Boolean
        Dim rectyp As Integer

        tinit = 1
        uunits = 1
        scnt = 0
        ReDim c(1)
        pastHeader = False
        Do
            retkey = -1
            If pUci.FastFlag Then
                GetNextRecordFromTable(blockname, tablename, srec, tinit, thisoccur, stemp, rectyp, retcod)
                'stemp = record returned
                'rectyp = record type returned, 0-normal, -1 comment, -2 blank
                'retcod = 1-returned header, 2-returned normal, 3-comment, 10-no more
            Else
                Call REM_XTABLEEX((Me.Uci), SCLU, SGRP, uunits, tinit, CInt(1), thisoccur, retkey, stemp, rectyp, retcod)
            End If
            tinit = 0
            If retcod = 2 Then
                'this is the type of record we want
                opf = CInt(Left(stemp, 5))
                'see if we already have a string with this oper
                sameoper = 0
                For i = 1 To scnt
                    If CDbl(Left(s(i), 5)) = opf Then
                        sameoper = i
                    End If
                Next i
                If sameoper = 0 Then
                    'this is a new operation
                    scnt = scnt + 1
                    ReDim Preserve s(scnt)
                    ReDim Preserve c(scnt + 1)
                    s(scnt) = stemp
                Else
                    'this is the same operation number, add to end for multiple line tables
                    If Len(s(sameoper)) < 80 Then
                        'pad with blanks
                        For i = (Len(s(sameoper)) + 1) To 80 'pad with blanks
                            s(sameoper) = s(sameoper) & " "
                        Next i
                    End If
                    s(sameoper) = s(sameoper) & Mid(stemp, 11)
                    For i = (Len(stemp) + 1) To 80 'pad with blanks
                        s(sameoper) = s(sameoper) & " "
                    Next i
                End If
            ElseIf retcod = 1 Then  'normal header ???
            ElseIf retcod = 3 Then  'comment
                If Len(c(scnt + 1)) = 0 Then
                    c(scnt + 1) = stemp
                Else
                    c(scnt + 1) = c(scnt + 1) & vbCrLf & stemp
                End If
            ElseIf retcod = 10 Then
                Exit Do
            Else
                Logger.Msg(stemp)
            End If
        Loop
    End Sub

    Public Sub CreateTables(ByVal aBlockDef As HspfBlockDef)
        Dim lTableNames() As String
        Static PERLNDtabList() As String = {"ACTIVITY", "PRINT-INFO", "GEN-INFO", "PWAT-PARM1", "PWAT-PARM2", "PWAT-PARM3", "PWAT-PARM4", "MON-INTERCEP", "MON-LZETPARM", "PWAT-STATE1"}
        Static IMPLNDtabList() As String = {"ACTIVITY", "PRINT-INFO", "GEN-INFO", "IWAT-PARM1", "IWAT-PARM2", "IWAT-PARM3", "IWAT-STATE1"}
        Static RCHREStabList() As String = {"ACTIVITY", "PRINT-INFO", "GEN-INFO", "HYDR-PARM1", "HYDR-PARM2", "HYDR-INIT"}

        'could do something here with table status info?
        Select Case aBlockDef.Name
            Case "PERLND"
                lTableNames = PERLNDtabList
            Case "IMPLND"
                lTableNames = IMPLNDtabList
            Case "RCHRES"
                lTableNames = RCHREStabList
            Case Else
                ReDim lTableNames(-1)
        End Select

        For Each lTableName As String In lTableNames
            For Each lOperation As HspfOperation In pIds
                Dim lTable As New HspfTable
                lTable.Opn = lOperation
                lTable.Def = aBlockDef.TableDefs.Item(lTableName)
                lTable.initTable("")
                lTable.OccurCount = 1
                lTable.OccurNum = 1
                lTable.Opn = lOperation
                lOperation.Tables.Add(lTable)
                If lTable.Name = "HYDR-PARM2" Then
                    lOperation.FTable = New HspfFtable
                    lOperation.FTable.Operation = lOperation
                    lTable.Parms("FTBUCI").Value = lOperation.Id
                    lOperation.FTable.Id = lOperation.Id
                End If
                If Not Me.TableExists(lTable.Name) Then
                    Me.Tables.Add(lTable) 'pbd - needs to be added?
                End If
            Next lOperation
        Next lTableName
    End Sub

    Public Sub AddTable(ByRef aOpId As Integer, _
                        ByRef aTableName As String, _
                        ByVal aBlockDef As HspfBlockDef)
        'add a table to the uci object for this operation id
        Dim s, t As String
        Dim O, i As Integer
        Dim lId As HspfOperation
        Dim vId As Object

        For Each vId In pIds
            lId = vId
            If lId.Id = aOpId Then
                Dim lTable As HspfTable = New HspfTable
                lTable.Opn = lId
                i = InStr(aTableName, ":")
                If i > 0 Then
                    t = Left(aTableName, i - 1)
                    O = CShort(Right(aTableName, Len(aTableName) - i))
                Else
                    t = aTableName
                    O = 1
                End If
                lTable.Def = aBlockDef.TableDefs.Item(t)
                s = ""
                lTable.initTable((s))
                lTable.OccurCount = O
                lTable.OccurNum = O
                If O > 1 Then
                    'set occurcounts for previous occurrances
                    If Me.TableExists(t) Then
                        Me.Tables.Item(t).OccurCount = O
                    End If
                    For i = 2 To O - 1
                        If Me.TableExists(t & ":" & i) Then
                            Me.Tables.Item(t & ":" & i).OccurCount = O
                        End If
                    Next i
                End If
                lTable.Opn = lId
                If Not lId.TableExists(aTableName) Then
                    lId.Tables.Add(lTable)
                End If
                If Not Me.TableExists(aTableName) Then
                    Me.Tables.Add(lTable)
                End If
                Exit For
            End If
        Next vId
    End Sub

    Public Sub AddTableForAll(ByRef aTableName As String, ByRef aOperationName As String)
        'add a table to the uci object for all operation ids
        Dim s, t As String
        Dim O, i As Integer
        Dim lBlockDef As HspfBlockDef = pUci.Msg.BlockDefs(aOperationName)

        For Each lOperation As HspfOperation In pIds
            Dim lTable As HspfTable = New HspfTable
            ltable.Opn = lOperation
            i = InStr(aTableName, ":")
            If i > 0 Then
                t = Left(aTableName, i - 1)
                O = CShort(Right(aTableName, Len(aTableName) - i))
            Else
                t = aTableName
                O = 1
            End If
            ltable.Def = lBlockDef.TableDefs.Item(t)
            s = ""
            ltable.initTable((s))
            ltable.OccurCount = O
            ltable.OccurNum = O
            If O > 1 Then
                'set occurcounts for previous occurrances
                If Me.TableExists(t) Then
                    Me.Tables.Item(t).OccurCount = O
                End If
                For i = 2 To O - 1
                    If Me.TableExists(t & ":" & i) Then
                        Me.Tables.Item(t & ":" & i).OccurCount = O
                    End If
                Next i
            End If
            ltable.Opn = lOperation
            If Not lOperation.TableExists(aTableName) Then 'add to this id
                lOperation.Tables.Add(ltable)
            End If
            If Not Me.TableExists(aTableName) Then 'add to this oper-type block
                Me.Tables.Add(ltable)
            End If
        Next lOperation
    End Sub

    Public Sub RemoveTable(ByRef aOperationId As Integer, ByRef aTableName As String)
        'remove this oper from a table, remove whole table if this is the last
        Dim lOperation As HspfOperation = OperFromID(aOperationId)
        lOperation.Tables.Remove(aTableName)
    End Sub

    Public Overrides Function ToString() As String
        Dim lSB As New StringBuilder
        If Comment.Length > 0 Then
            lSB.AppendLine(Comment)
        End If
        lSB.AppendLine(Me.Name)

        Dim lGroupIndex, lFirstInGroup As Integer
        Dim lLastInGroup, lLastGroupIndex, lCurrentOccurGroup As Integer
        Dim lInGroup As Boolean = False
        Dim lBlockDef As HspfBlockDef = pUci.Msg.BlockDefs.Item(Me.Name)
        Dim lTableDefIndex As Integer = 0
        Do While lTableDefIndex < lBlockDef.TableDefs.Count
            'must look thru all possible tables
            lTableDefIndex += 1
            Dim lTableDef As HspfTableDef = lBlockDef.TableDefs(lTableDefIndex - 1)

            If lTableDef.OccurGroup = 0 And Not lInGroup Then 'the basic case
                For Each lOperation As HspfOperation In pIds
                    If lOperation.TableExists(lTableDef.Name) Then
                        Dim lTable As HspfTable = lOperation.Tables.Item(lTableDef.Name)
                        If lTable.TableComment.Length > 0 Then
                            lSB.AppendLine(lTable.TableComment)
                        End If
                        lSB.AppendLine(lTable.ToString) 'this writes all like this
                        Exit For
                    End If
                Next lOperation
            Else 'this is a multiple occurence group (like pqual, iqual, gqual)
                If lInGroup Then
                    If lTableDef.OccurGroup <> lCurrentOccurGroup Or lTableDefIndex = lBlockDef.TableDefs.Count Then
                        'we were in a multiple occurence group but have reached end of group
                        lGroupIndex += 1 'look for next occurence
                        If lGroupIndex > lLastGroupIndex Then
                            lInGroup = False 'no more to do
                            If lLastInGroup > 0 Then
                                lTableDefIndex = lLastInGroup
                            End If
                        Else
                            lLastInGroup = lTableDefIndex - 1 'remember which was the last table in group
                            lTableDefIndex = lFirstInGroup
                            lTableDef = lBlockDef.TableDefs.Item(lTableDefIndex - 1)
                        End If
                    End If
                Else 'start of a multiple occurence group
                    lInGroup = True
                    lGroupIndex = 1
                    lFirstInGroup = lTableDefIndex
                    lLastGroupIndex = 0
                    lLastInGroup = 0
                    lCurrentOccurGroup = lTableDef.OccurGroup
                End If

                If lInGroup Then
                    For Each lOperation As HspfOperation In pIds
                        'If lId.TableExists(lTableDef.Name) Then  'accomodate empty placeholder tables
                        If lOperation.OpnBlk.TableExists((lTableDef.Name)) Then
                            Dim lTable As HspfTable = lOperation.OpnBlk.Tables(lTableDef.Name)
                            If lTable.OccurIndex = 0 Or _
                              (lTable.OccurIndex > 0 And lTable.OccurIndex <= lGroupIndex) Then
                                If lGroupIndex > 1 And lTable.OccurIndex = 0 Then
                                    'write the comment that applies to this table
                                    Dim lTableKey As String = lTable.Name & ":" & lGroupIndex
                                    Dim lTableX As HspfTable = lOperation.OpnBlk.Tables(lTableKey)
                                    If lTableX.TableComment.Length > 0 Then
                                        lSB.AppendLine(lTableX.TableComment)
                                    End If
                                Else
                                    If lTable.TableComment.Length > 0 Then
                                        lSB.AppendLine(lTable.TableComment)
                                    End If
                                End If
                                If lTable.OccurIndex = 0 Then 'write out just this occurence
                                    lSB.AppendLine(lTable.ToStringByIndex(lGroupIndex))
                                Else
                                    'special case for some p/i/gqual tables
                                    Dim j As Integer = 0
                                    For lTableOccurIndex As Integer = 1 To lTable.OccurCount
                                        Dim lTableKey As String = lTable.Name
                                        If lTableOccurIndex > 1 Then
                                            lTableKey &= ":" & lTableOccurIndex
                                        End If
                                        Dim lTableX As HspfTable = lOperation.OpnBlk.Tables(lTableKey)
                                        If lTableX.OccurIndex = lGroupIndex Then
                                            j = lTableX.OccurNum
                                            Exit For
                                        End If
                                    Next lTableOccurIndex
                                    If j > 0 Then 'write out just this occurence
                                        lSB.AppendLine(lTable.ToStringByIndex(j))
                                    End If
                                End If
                                If lTable.OccurCount > lLastGroupIndex Then
                                    lLastGroupIndex = lTable.OccurCount
                                End If
                                Exit For
                            End If
                        End If
                    Next lOperation
                End If
            End If
        Loop

        lSB.AppendLine("END " & Me.Name)
        Return lSB.ToString
    End Function

    Public Function OperByDesc(ByRef Desc As String) As HspfOperation
        Dim lId As HspfOperation
        Dim vId As Object

        'UPGRADE_NOTE: Object OperByDesc may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSExpressCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        OperByDesc = Nothing 'Changed by Mark
        For Each vId In Ids
            lId = vId
            If lId.Description = Desc Then
                OperByDesc = lId 'Changed by Mark
                Exit For
            End If
        Next vId
    End Function

    Private Sub SortIntegerArray(ByRef opt As Integer, ByRef cnt As Integer, ByRef iVal() As Integer, ByRef pos() As Integer)
        ' ##SUMMARY Sorts integers in array into ascending order.
        ' ##PARAM opt I Sort option (0 = sort in place, 1 = move values in array to sorted position)
        ' ##PARAM cnt I Count of integers to sort
        ' ##PARAM iVal I Array of integers to sort
        ' ##PARAM pos O Array containing sorted order of integers
        Dim i As Integer
        Dim j As Integer
        Dim jpt As Integer
        Dim jpt1 As Integer
        Dim itmp As Integer
        ' ##LOCAL i - long counter for outer loop
        ' ##LOCAL j - long counter for inner loop
        ' ##LOCAL jpt - long pointer to j index
        ' ##LOCAL jpt1 - long pointer to (j + 1) index
        ' ##LOCAL itmp - long temporary holder for values in iVal array

        'set default positions(assume in order)
        For i = 1 To cnt
            pos(i) = i
        Next i

        'make a pointer to values with bubble sort
        For i = cnt To 2 Step -1
            For j = 1 To i - 1
                jpt = pos(j)
                jpt1 = pos(j + 1)
                If (iVal(jpt) > iVal(jpt1)) Then
                    pos(j + 1) = jpt
                    pos(j) = jpt1
                End If
            Next j
        Next i

        If (opt = 1) Then
            'move integer values to their sorted positions
            For i = 1 To cnt
                If (pos(i) <> i) Then
                    'need to move ints, first save whats in target space
                    itmp = iVal(i)
                    'move sorted data to target position
                    iVal(i) = iVal(pos(i))
                    'move temp data to source position
                    iVal(pos(i)) = itmp
                    'find the pointer to the other value we are moving
                    j = i
                    Do
                        j = j + 1
                    Loop While (pos(j) <> i)
                    pos(j) = pos(i)
                    pos(i) = i
                End If
            Next i
        End If
    End Sub

End Class