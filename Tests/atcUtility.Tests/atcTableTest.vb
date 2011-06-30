﻿Imports System.Collections

Imports System.Reflection

Imports System

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports atcUtility



'''<summary>
'''This is a test class for atcTableTest and is intended
'''to contain all atcTableTest Unit Tests
'''</summary>
<TestClass()> _
Public Class atcTableTest


    Private testContextInstance As TestContext

    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = Value
        End Set
    End Property

#Region "Additional test attributes"
    '
    'You can use the following additional attributes as you write your tests:
    '
    'Use ClassInitialize to run code before running the first test in the class
    '<ClassInitialize()>  _
    'Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
    'End Sub
    '
    'Use ClassCleanup to run code after all tests in a class have run
    '<ClassCleanup()>  _
    'Public Shared Sub MyClassCleanup()
    'End Sub
    '
    'Use TestInitialize to run code before running each test
    '<TestInitialize()>  _
    'Public Sub MyTestInitialize()
    'End Sub
    '
    'Use TestCleanup to run code after each test has run
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region


    Friend Overridable Function CreateatcTable() As atcTable
        'TODO: Instantiate an appropriate concrete class.
        Dim target As atcTable = Nothing
        Return target
    End Function

    '''<summary>
    '''A test for Clear
    '''</summary>
    <TestMethod()> _
    Public Sub ClearTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        target.Clear()
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for ClearData
    '''</summary>
    <TestMethod()> _
    Public Sub ClearDataTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        target.ClearData()
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for ComputeFieldLengths
    '''</summary>
    <TestMethod()> _
    Public Sub ComputeFieldLengthsTest()
        Dim target As atcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected() As Integer = Nothing ' TODO: Initialize to an appropriate value
        Dim actual() As Integer
        actual = target.ComputeFieldLengths
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for Cousin
    '''</summary>
    <TestMethod()> _
    Public Sub CousinTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As IatcTable = Nothing ' TODO: Initialize to an appropriate value
        Dim actual As IatcTable
        actual = target.Cousin
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for CreationCode
    '''</summary>
    <TestMethod()> _
    Public Sub CreationCodeTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        actual = target.CreationCode
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for CurrentRecordAsDelimitedString
    '''</summary>
    <TestMethod()> _
    Public Sub CurrentRecordAsDelimitedStringTest()
        Dim target As atcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aDelimiter As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim aQuote As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        actual = target.CurrentRecordAsDelimitedString(aDelimiter, aQuote)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for EOF
    '''</summary>
    <TestMethod()> _
    Public Sub EOFTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As Boolean = False ' TODO: Initialize to an appropriate value
        Dim actual As Boolean
        actual = target.EOF
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FieldNumber
    '''</summary>
    <TestMethod()> _
    Public Sub FieldNumberTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldName As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim actual As Integer
        actual = target.FieldNumber(aFieldName)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FindFirst
    '''</summary>
    <TestMethod()> _
    Public Sub FindFirstTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNumber As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim aFindValue As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim aStartRecord As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim aEndRecord As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As Boolean = False ' TODO: Initialize to an appropriate value
        Dim actual As Boolean
        actual = target.FindFirst(aFieldNumber, aFindValue, aStartRecord, aEndRecord)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FindMatch
    '''</summary>
    <TestMethod()> _
    Public Sub FindMatchTest()
        Dim target As atcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNum() As Integer = Nothing ' TODO: Initialize to an appropriate value
        Dim aOperation() As String = Nothing ' TODO: Initialize to an appropriate value
        Dim aFieldVal() As Object = Nothing ' TODO: Initialize to an appropriate value
        Dim aMatchAny As Boolean = False ' TODO: Initialize to an appropriate value
        Dim aStartRecord As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim aEndRecord As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As Boolean = False ' TODO: Initialize to an appropriate value
        Dim actual As Boolean
        actual = target.FindMatch(aFieldNum, aOperation, aFieldVal, aMatchAny, aStartRecord, aEndRecord)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FindNext
    '''</summary>
    <TestMethod()> _
    Public Sub FindNextTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNumber As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim aFindValue As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As Boolean = False ' TODO: Initialize to an appropriate value
        Dim actual As Boolean
        actual = target.FindNext(aFieldNumber, aFindValue)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for MoveFirst
    '''</summary>
    <TestMethod()> _
    Public Sub MoveFirstTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        target.MoveFirst()
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for MoveLast
    '''</summary>
    <TestMethod()> _
    Public Sub MoveLastTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        target.MoveLast()
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for MoveNext
    '''</summary>
    <TestMethod()> _
    Public Sub MoveNextTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        target.MoveNext()
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for MovePrevious
    '''</summary>
    <TestMethod()> _
    Public Sub MovePreviousTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        target.MovePrevious()
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for OpenFile
    '''</summary>
    <TestMethod()> _
    Public Sub OpenFileTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFileName As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As Boolean = False ' TODO: Initialize to an appropriate value
        Dim actual As Boolean
        actual = target.OpenFile(aFileName)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for PopulateObject
    '''</summary>
    <TestMethod()> _
    Public Sub PopulateObjectTest()
        Dim target As atcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aObject As Object = Nothing ' TODO: Initialize to an appropriate value
        Dim aObjectExpected As Object = Nothing ' TODO: Initialize to an appropriate value
        Dim aFieldMap As atcCollection = Nothing ' TODO: Initialize to an appropriate value
        target.PopulateObject(aObject, aFieldMap)
        Assert.AreEqual(aObjectExpected, aObject)
        Assert.Inconclusive("A method that does not return a value cannot be verified.")
    End Sub

    '''<summary>
    '''A test for PopulateObjects
    '''</summary>
    <TestMethod()> _
    Public Sub PopulateObjectsTest()
        Dim target As atcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aObjectType As Type = Nothing ' TODO: Initialize to an appropriate value
        Dim aFieldMap As atcCollection = Nothing ' TODO: Initialize to an appropriate value
        Dim aNewArgs() As Object = Nothing ' TODO: Initialize to an appropriate value
        Dim aNewBindingFlags As BindingFlags = New BindingFlags() ' TODO: Initialize to an appropriate value
        Dim expected As ArrayList = Nothing ' TODO: Initialize to an appropriate value
        Dim actual As ArrayList
        actual = target.PopulateObjects(aObjectType, aFieldMap, aNewArgs, aNewBindingFlags)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for Summary
    '''</summary>
    <TestMethod()> _
    Public Sub SummaryTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFormat As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        actual = target.Summary(aFormat)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for SummaryFields
    '''</summary>
    <TestMethod()> _
    Public Sub SummaryFieldsTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFormat As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        actual = target.SummaryFields(aFormat)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for SummaryFile
    '''</summary>
    <TestMethod()> _
    Public Sub SummaryFileTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFormat As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        actual = target.SummaryFile(aFormat)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for TrimValue
    '''</summary>
    <TestMethod()> _
    Public Sub TrimValueTest()
        Dim target As atcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aValue As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim aType As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        actual = target.TrimValue(aValue, aType)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for WriteFile
    '''</summary>
    <TestMethod()> _
    Public Sub WriteFileTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFileName As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim expected As Boolean = False ' TODO: Initialize to an appropriate value
        Dim actual As Boolean
        actual = target.WriteFile(aFileName)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for CurrentRecord
    '''</summary>
    <TestMethod()> _
    Public Sub CurrentRecordTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim actual As Integer
        target.CurrentRecord = expected
        actual = target.CurrentRecord
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FieldLength
    '''</summary>
    <TestMethod()> _
    Public Sub FieldLengthTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNumber As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim actual As Integer
        target.FieldLength(aFieldNumber) = expected
        actual = target.FieldLength(aFieldNumber)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FieldName
    '''</summary>
    <TestMethod()> _
    Public Sub FieldNameTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNumber As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        target.FieldName(aFieldNumber) = expected
        actual = target.FieldName(aFieldNumber)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FieldType
    '''</summary>
    <TestMethod()> _
    Public Sub FieldTypeTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNumber As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        target.FieldType(aFieldNumber) = expected
        actual = target.FieldType(aFieldNumber)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for FileName
    '''</summary>
    <TestMethod()> _
    Public Sub FileNameTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        target.FileName = expected
        actual = target.FileName
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for Header
    '''</summary>
    <TestMethod()> _
    Public Sub HeaderTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aHeaderRow As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        target.Header(aHeaderRow) = expected
        actual = target.Header(aHeaderRow)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for Header
    '''</summary>
    <TestMethod()> _
    Public Sub HeaderTest1()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aHeaderRow As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        target.Header(aHeaderRow) = expected
        actual = target.Header(aHeaderRow)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for NumFields
    '''</summary>
    <TestMethod()> _
    Public Sub NumFieldsTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim actual As Integer
        target.NumFields = expected
        actual = target.NumFields
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for NumHeaderRows
    '''</summary>
    <TestMethod()> _
    Public Sub NumHeaderRowsTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim actual As Integer
        target.NumHeaderRows = expected
        actual = target.NumHeaderRows
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for NumRecords
    '''</summary>
    <TestMethod()> _
    Public Sub NumRecordsTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim expected As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim actual As Integer
        target.NumRecords = expected
        actual = target.NumRecords
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub

    '''<summary>
    '''A test for Value
    '''</summary>
    <TestMethod()> _
    Public Sub ValueTest()
        Dim target As IatcTable = CreateatcTable() ' TODO: Initialize to an appropriate value
        Dim aFieldNumber As Integer = 0 ' TODO: Initialize to an appropriate value
        Dim expected As String = String.Empty ' TODO: Initialize to an appropriate value
        Dim actual As String
        target.Value(aFieldNumber) = expected
        actual = target.Value(aFieldNumber)
        Assert.AreEqual(expected, actual)
        Assert.Inconclusive("Verify the correctness of this test method.")
    End Sub
End Class