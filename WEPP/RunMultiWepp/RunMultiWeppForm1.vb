﻿Imports System.IO


Public Class RunMultiWeppForm1

    Public lSlopeIterations As Integer
    Public lLengthIterations As Integer
    Public lSlopeDelta As Double
    Public lLengthDelta As Double
    Public lRunCount As Integer = 0

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Temporary Default Value set
        txtPathBase.Text = "Z:\Documents\filecabinet\employment\aquaterra\active.projects\SERDP\Roads\WEPP\cli.met\4\in.slp"
        txtPathSlope.Text = "Z:\Documents\filecabinet\employment\aquaterra\active.projects\SERDP\Roads\WEPP\wepp.run\in.slp"
        txtPathWepp.Text = "Z:\Documents\filecabinet\employment\aquaterra\active.projects\SERDP\Roads\WEPP\wepp.run"
        txtPathPlot.Text = "Z:\Documents\filecabinet\employment\aquaterra\active.projects\SERDP\Roads\WEPP\wepp.run\out-plot.txt"
        txtPathOutput.Text = "Z:\Documents\filecabinet\employment\aquaterra\active.projects\SERDP\Roads\WEPP\sensitivity\A"

        PopulateCopyAlso()

        txtSlopeStart.Text = "1"
        txtSlopeStop.Text = "40"
        txtSlopeDelta.Text = "1"
        
        txtLengthStart.Text = "1"
        txtLengthStop.Text = "300"
        txtLengthDelta.Text = "20"
        
        radioOFERoad.Checked = True

        AddHandler chkEnableAlso.CheckStateChanged, AddressOf PopulateCopyAlso
        AddHandler txtSlopeStart.TextChanged, AddressOf ParamTextChanged
        AddHandler txtSlopeStop.TextChanged, AddressOf ParamTextChanged
        AddHandler txtSlopeDelta.TextChanged, AddressOf ParamTextChanged
        AddHandler txtLengthStart.TextChanged, AddressOf ParamTextChanged
        AddHandler txtLengthStop.TextChanged, AddressOf ParamTextChanged
        AddHandler txtLengthDelta.TextChanged, AddressOf ParamTextChanged
        
        CalculateIterations()

    End Sub
    Private Function GetBaseDir() As String
        Dim lString As String = ""

        If txtPathBase.Text.Length > 1 Then
            lString = Microsoft.VisualBasic.Left(txtPathBase.Text, txtPathBase.Text.LastIndexOf("\")) & "\"
        End If

        Return lString
    End Function

    Private Sub PopulateCopyAlso()
        Try

            If chkEnableAlso.Checked Then

                lstCopyAlso.Enabled = True

            'copy other files from base to wepp executable if they are selected
            Dim lObjBaseDir As Directory
                Dim lBaseFileList() As String = lObjBaseDir.GetFiles(GetBaseDir, "*")
            Dim lOper As String
            Dim lSingleFileName As String
            Dim lAutoDetect As Boolean

                For Each lOper In lBaseFileList
                    If lOper <> txtPathBase.Text Then
                        lSingleFileName = Microsoft.VisualBasic.Right(lOper, lOper.Length - lOper.LastIndexOf("\") - 1)
                        If Microsoft.VisualBasic.Right(lSingleFileName, 3) = "man" Or Microsoft.VisualBasic.Right(lSingleFileName, 3) = "cli" Or Microsoft.VisualBasic.Right(lSingleFileName, 3) = "run" Or Microsoft.VisualBasic.Right(lSingleFileName, 3) = "sol" Then lAutoDetect = True
                        lstCopyAlso.Items.Add(lSingleFileName, lAutoDetect)
                        lAutoDetect = False
                    End If
                Next
            Else
                lstCopyAlso.Items.Clear()
                lstCopyAlso.Enabled = False
            End If

        Catch ex As Exception

        End Try
    End Sub
    Private Sub btnBasePath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBasePath.Click
        Try
            OpenFileDialog1.ShowDialog()

            If Not OpenFileDialog1.FileName Is Nothing Then
                txtPathBase.Text = OpenFileDialog1.FileName
            End If

        Catch ex As Exception
            MsgBox("Bad Path Name")
        End Try

    End Sub

    Private Sub btnWeppPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWeppPath.Click
        Try
            OpenFileDialog1.Reset()
            OpenFileDialog1.ShowDialog()

            If Not OpenFileDialog1.FileName Is Nothing Then
                txtPathWepp.Text = OpenFileDialog1.FileName
            End If

        Catch ex As Exception
            MsgBox("Bad Path Name")
        End Try
    End Sub

    Private Sub btnSlopePath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSlopePath.Click
        Try
            OpenFileDialog1.Reset()
            OpenFileDialog1.ShowDialog()

            If Not OpenFileDialog1.FileName Is Nothing Then
                txtPathSlope.Text = OpenFileDialog1.FileName
            End If

        Catch ex As Exception
            MsgBox("Bad Path Name")
        End Try
    End Sub

    Private Sub btnPathPlot_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPathPlot.Click
        Try
            OpenFileDialog1.Reset()
            OpenFileDialog1.ShowDialog()

            If Not OpenFileDialog1.FileName Is Nothing Then
                txtPathPlot.Text = OpenFileDialog1.FileName
            End If

        Catch ex As Exception
            MsgBox("Bad Path Name")
        End Try
    End Sub

    Private Sub btnOutPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOutPath.Click
        Try
            OpenFileDialog1.Reset()
            OpenFileDialog1.ShowDialog()

            If Not OpenFileDialog1.FileName Is Nothing Then
                txtPathOutput.Text = OpenFileDialog1.FileName
            End If

        Catch ex As Exception
            MsgBox("Bad Path Name")
        End Try
    End Sub


    Function WEPPformatMoreDetail(ByVal aNumber As Double) As String
        Dim lStr As String = Math.Round(aNumber, 2, MidpointRounding.AwayFromZero)
        Return lStr
    End Function

    Private Sub btnExecute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecute.Click
        txtRunStatus.Text = "Running..."
        btnExecute.Enabled = False
        btnExecute.Text = "Running..."

        Dim lMainStepIndex As Integer = -1

        Dim lCurrentSlope As Double = CDbl(txtLengthStart.Text)
        Dim lCurrentLength As Double = CDbl(txtLengthStart.Text)

        Try

            'copy additional files if any were selected
            If lstCopyAlso.CheckedItems.Count > 1 Then
                For Each lObj In lstCopyAlso.CheckedItems
                    System.IO.File.Copy(GetBaseDir() & lObj, txtPathWepp.Text & "\" & lObj, True)
                Next
            End If

            'Remove other files in out directory
            If chkDelAll.Checked Then

                For Each lFileFound As String In Directory.GetFiles(txtPathOutput.Text, "*.*")
                    File.Delete(lFileFound)
                Next

            End If

            'Read the source slope file (once)
            Dim objReader As New System.IO.StreamReader(txtPathBase.Text)
            Dim lLinesSlopeIn As New ArrayList

            Do While objReader.Peek() <> -1
                lLinesSlopeIn.Add(objReader.ReadLine())
            Loop

            'Make a text file linking main step index to parameter values. Plot files are names after main step index.
            If Not System.IO.File.Exists(txtPathOutput.Text & "\index.csv") Then System.IO.File.Create(txtPathOutput.Text & "\index.csv", 1)

            Dim objIndexWriter As New System.IO.StreamWriter(txtPathOutput.Text & "\index.csv")
            objIndexWriter.WriteLine("# Index of multiple slope vs. length WEPP:Road runs. (index,slope(%),length(m)")


            For j = 0 To lLengthIterations - 1 ' Step length
                lCurrentLength = CDbl(txtLengthStart.Text) + j * lLengthDelta
                For k = 0 To lSlopeIterations - 1 'Step slope (first)
                    lCurrentSlope = CDbl(txtSlopeStart.Text) + k * lSlopeDelta

                                                            'Copy the master slope file input to a local copy to muck with
                    Dim lLinesCurrentSlope As ArrayList = lLinesSlopeIn

                    If radioOFERoad.Checked Then


                        'LINE 5 !!!! Get the new road length
                        lLinesCurrentSlope(4) = "2 " & lCurrentLength * 0.3048

                        'LINE 6 !!!! Read line 6 and separate the elements by delimiter of comma

                        Dim lLine6Elements() As String = lLinesSlopeIn(5).ToString.Split(" ")
                        lLine6Elements(1) = WEPPformatMoreDetail(lCurrentSlope / 100)
                        lLine6Elements(4) = WEPPformatMoreDetail(lCurrentSlope / 100)

                        lLinesCurrentSlope(5) = ""
                        For i = 0 To lLine6Elements.Length - 1
                            lLinesCurrentSlope(5) &= lLine6Elements(i)
                            If Not i = lLine6Elements.Length - 1 Then lLinesCurrentSlope(5) &= " "
                        Next

                        'LINE 8 !!!! Read line 8 and separate the elements by delimiter of comma

                        Dim lLine8Elements() As String = lLinesSlopeIn(7).ToString.Split(" ")
                        lLine8Elements(1) = WEPPformatMoreDetail(lCurrentSlope / 100)

                        lLinesCurrentSlope(7) = ""
                        For i = 0 To lLine8Elements.Length - 1
                            lLinesCurrentSlope(7) &= lLine8Elements(i)
                            If Not i = lLine8Elements.Length - 1 Then lLinesCurrentSlope(7) &= " "
                        Next


                    ElseIf radioOFEBuffer.Checked Then

                        'LINE 9 !!!! Get the new road length
                        lLinesCurrentSlope(8) = "3 " & lCurrentLength * 0.3048

                        'LINE 10 !!!! Read line 10 and separate the elements by delimiter of comma

                        Dim lLine10Elements() As String = lLinesSlopeIn(9).ToString.Split(" ")

                        lLine10Elements(4) = WEPPformatMoreDetail(lCurrentSlope / 100)
                        lLine10Elements(7) = WEPPformatMoreDetail(lCurrentSlope / 100)

                        lLinesCurrentSlope(9) = ""
                        For i = 0 To lLine10Elements.Length - 1
                            lLinesCurrentSlope(9) &= lLine10Elements(i)
                            If Not i = lLine10Elements.Length - 1 Then lLinesCurrentSlope(9) &= " "
                        Next



                    End If

                    'Write the modified Slope File
                    If Not System.IO.File.Exists(txtPathSlope.Text) Then System.IO.File.Create(txtPathSlope.Text, 1)

                    Dim objWriter As New System.IO.StreamWriter(txtPathSlope.Text)

                    For i = 0 To lLinesSlopeIn.Count - 1
                        objWriter.WriteLine(lLinesCurrentSlope(i))
                    Next

                    objWriter.Close()

                    'note that the actual index (that is references) is one more than this, a -1 indicates that the first try of executing wepp was not successfull.
                    'could do this with exit code and another index, but not able to turn the pesky Vista security warnings off when executing an external shell command in a more contemporary way.
                    lMainStepIndex += 1

                    'write line in comma separated format for this index indicating parameter values
                    objIndexWriter.WriteLine(lMainStepIndex + 1 & "," & lCurrentSlope & "," & lCurrentLength)

                    txtRunStatus.Text = "Running: " & lMainStepIndex + 1 & "/" & lRunCount.ToString
                    ProgressBar1.Value = Math.Round((lMainStepIndex) / lRunCount) * 100
                    Shell(txtPathWepp.Text & "\weppbat.bat", AppWinStyle.Hide, True)

                    If System.IO.File.Exists(txtPathPlot.Text) Then System.IO.File.Copy(txtPathPlot.Text, txtPathOutput.Text & "\" & lMainStepIndex + 1 & ".txt", True)

                Next

            Next

            

            objIndexWriter.Close()

            ProgressBar1.Value = 100
            txtRunStatus.Text = "Done!"

            btnExecute.Enabled = True
            btnExecute.Text = "Run"
        Catch ex As Exception
            If lMainStepIndex > -1 Then
                txtRunStatus.Text = "Error: Last attempted run was at index " & lMainStepIndex & "(Slope = " & lCurrentSlope & ", Length = " & lCurrentLength & ".)"
            End If
            txtRunStatus.Text = "Error"
            MsgBox("Something(s) Bad, Check Input, paths and Wepp error file")
            btnExecute.Enabled = True
        End Try
    End Sub

    Private Sub CalculateIterations()
        'Calculate the number of times to interate based on both length and slope variation
        txtRunStatus.Text = "<Press Run>"

        Dim lSlopeStart As Double = CDbl(txtSlopeStart.Text)
        Dim lSlopeStop As Double = CDbl(txtSlopeStop.Text)

        Dim lLengthStart As Double = CDbl(txtLengthStart.Text)
        Dim lLengthStop As Double = CDbl(txtLengthStop.Text)

        lSlopeDelta = CDbl(txtSlopeDelta.Text)
        lLengthDelta = CDbl(txtLengthDelta.Text)

        lSlopeIterations = Math.Floor((lSlopeStop - lSlopeStart) / (lSlopeDelta) + 1)
        lLengthIterations = Math.Floor((lLengthStop - lLengthStart) / (lLengthDelta) + 1)

        lRunCount = CInt(lSlopeIterations * lLengthIterations)
        txtRunCount.Text = lRunCount.ToString

        If lRunCount > 0 AndAlso Math.IEEERemainder(lRunCount, 1) = 0 Then
            btnExecute.Enabled = True
            btnExecute.Text = "Run"
            txtRunCount.Text = lRunCount
        Else
            btnExecute.Enabled = False
            btnExecute.Text = "Input Error :-("
        End If


    End Sub

    Private Sub ParamTextChanged(ByVal aSender As System.Object, ByVal aEventArgs As System.EventArgs)
        Dim lLocalControl As New System.Windows.Forms.TextBox
        lLocalControl = aSender

        If IsNumeric(lLocalControl.Text) Then
            CalculateIterations()
        Else
            lRunCount = 0
            txtRunCount.Text = "-"
            btnExecute.Enabled = False
            btnExecute.Text = "Bad Input"
        End If

    End Sub
    
    Private Sub txtPathBase_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        GetBaseDir()
    End Sub

    Private Sub radioOFEBuffer_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioOFEBuffer.CheckedChanged
        txtSlopeStart.Text = "1"
        txtSlopeStop.Text = "100"
        txtSlopeDelta.Text = "5"

        txtLengthStart.Text = "1"
        txtLengthStop.Text = "300"
        txtLengthDelta.Text = "20"
    End Sub

    Private Sub radioOFERoad_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radioOFERoad.CheckedChanged
        txtSlopeStart.Text = "1"
        txtSlopeStop.Text = "40"
        txtSlopeDelta.Text = "2"

        txtLengthStart.Text = "1"
        txtLengthStop.Text = "300"
        txtLengthDelta.Text = "20"
    End Sub

    Private Sub RunMultiWeppForm1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class