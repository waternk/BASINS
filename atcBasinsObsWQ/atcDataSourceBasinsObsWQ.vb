Option Strict Off
Option Explicit On 

Imports atcData
Imports atcUtility

Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.IO

Public Class atcDataSourceBasinsObsWQ
  Inherits atcDataSource
  '##MODULE_REMARKS Copyright 2005 AQUA TERRA Consultants - Royalty-free use permitted under open source license

  Private Shared pFileFilter As String = "Basins Observed WQ Files (*.dbf)|*.dbf"
  Private pErrorDescription As String
  Private pColDefs As Hashtable
  'Private pReadAll As Boolean = False

  Public Overrides ReadOnly Property Description() As String
    Get
      Return "Basins Observed Water Quality DBF"
    End Get
  End Property

  Public Overrides ReadOnly Property Name() As String
    Get
      Return "Timeseries::BasinsObsWQ"
    End Get
  End Property

  Public Overrides ReadOnly Property Category() As String
    Get
      Return "File"
    End Get
  End Property

  Public Overrides ReadOnly Property CanOpen() As Boolean
    Get
      Return True 'yes, this class can open files
    End Get
  End Property

  Public Overrides ReadOnly Property CanSave() As Boolean
    Get
      Return False 'no saving yet, but could implement if needed 
    End Get
  End Property

  Public Overrides Function Open(ByVal aFileName As String, Optional ByVal aAttributes As atcData.atcDataAttributes = Nothing) As Boolean
    Dim lData As atcTimeseries
    Dim lDates As atcTimeseries

    If aFileName Is Nothing OrElse aFileName.Length = 0 OrElse Not FileExists(aFileName) Then
      aFileName = FindFile("Select " & Name & " file to open", , , pFileFilter, True, , 1)
    End If

    If Not FileExists(aFileName) Then
      pErrorDescription = "File '" & aFileName & "' not found"
    Else
      Me.Specification = aFileName

      Try
        Dim lDBF As IATCTable
        Dim lDateCol As Integer = -1
        Dim lTimeCol As Integer = -1
        Dim lLocnCol As Integer = -1
        Dim lConsCol As Integer = -1
        Dim lValCol As Integer = -1
        Dim lTSKey As String
        Dim lTSIndex As Integer
        Dim lLocation As String = ""
        Dim lConstituentCode As Integer = -1
        Dim s As String
        lDBF = New atcTableDBF
        lDBF.OpenFile(aFileName)
        For i As Integer = 1 To lDBF.NumFields
          s = UCase(lDBF.FieldName(i))
          If s = "DATE" Then
            lDateCol = i
          ElseIf s = "TIME" Then
            lTimeCol = i
          ElseIf InStr(s, "ID") Then 'location
            If lLocnCol = -1 Then 'only use first one
              'should be sure that field is in use here
              lLocnCol = i
            End If
          ElseIf s = "PARM" Then
            lConsCol = i
          ElseIf s = "VALUE" Then
            lValCol = i
          End If
        Next
        If lDateCol > 0 AndAlso lTimeCol > 0 AndAlso lLocnCol > 0 AndAlso _
           lConsCol > 0 AndAlso lValCol > 0 Then
          While Not lDBF.atEOF
            lLocation = lDBF.Value(lLocnCol)
            lConstituentCode = lDBF.Value(lConsCol)
            lTSKey = lLocation & ":" & lConstituentCode
            lData = DataSets.ItemByKey(lTSKey)
            If lData Is Nothing Then
              lData = New atcTimeseries(Me)
              lData.Dates = New atcTimeseries(Me)
              lData.numValues = lDBF.NumRecords - lDBF.CurrentRecord + 1
              lData.Value(0) = Double.NaN
              lData.Dates.Value(0) = Double.NaN
              lData.Attributes.SetValue("Count", 0)
              lData.Attributes.SetValue("Scenario", "OBSERVED")
              lData.Attributes.SetValue("Location", lLocation)
              lData.Attributes.SetValue("Constituent", lConstituentCode)
              lData.Attributes.SetValue("Point", True)
              DataSets.Add(lTSKey, lData)
            End If
            lTSIndex = lData.Attributes.GetValue("Count") + 1
            lData.Value(lTSIndex) = lDBF.Value(lValCol)
            lData.Dates.Value(lTSIndex) = parseWQObsDate(lDBF.Value(lDateCol), lDBF.Value(lTimeCol))
            lData.Attributes.SetValue("Count", lTSIndex)
            lDBF.MoveNext()
          End While
          For Each lData In DataSets
            lData.numValues = lData.Attributes.GetValue("Count")
          Next
          Open = True
        End If
      Catch endEx As EndOfStreamException
        Open = False
      End Try
    End If
  End Function

  Private Function parseWQObsDate(ByVal aDate As String, ByVal aTime As String) As Double
    'assume point values at specified time
    Dim d(5) As Integer 'date array
    Dim l As Integer 'Length of year (2 or 4 digit year)
    Dim i As Integer 'Year offset (1900 for 2-digit year)

    If Not IsNumeric(aTime) Then aTime = "1200" 'assume noon for missing obstime
    If IsNumeric(aDate) Then
      If Len(aDate) = 8 Then ' 4 dig yr
        l = 4
        i = 0
      Else
        l = 2
        i = 1900
      End If
      d(0) = Left(aDate, l) + i
      d(1) = Mid(aDate, l + 1, 2)
      d(2) = Right(aDate, 2)
      If IsNumeric(aTime) Then
        d(3) = Left(aTime, 2)
        d(4) = Right(aTime, 2)
      End If
      Return Date2J(d)
    Else
      Return 0
    End If
  End Function

End Class