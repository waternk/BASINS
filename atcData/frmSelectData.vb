Imports atcData

Imports System.Windows.Forms

Friend Class frmSelectData
  Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

  Public Sub New()
    MyBase.New()

    InitializeComponent()

    pMatchingGrid.AllowHorizontalScrolling = False
    pSelectedGrid.AllowHorizontalScrolling = False

  End Sub

  'Form overrides dispose to clean up the component list.
  Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      If Not (components Is Nothing) Then
        components.Dispose()
      End If
    End If
    MyBase.Dispose(disposing)
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  Friend WithEvents groupTop As System.Windows.Forms.GroupBox
  Friend WithEvents pnlButtons As System.Windows.Forms.Panel
  Friend WithEvents btnOk As System.Windows.Forms.Button
  Friend WithEvents btnCancel As System.Windows.Forms.Button
  Friend WithEvents splitAboveSelected As System.Windows.Forms.Splitter
  Friend WithEvents groupSelected As System.Windows.Forms.GroupBox
  Friend WithEvents panelCriteria As System.Windows.Forms.Panel
  Friend WithEvents splitAboveMatching As System.Windows.Forms.Splitter
  Friend WithEvents lblMatching As System.Windows.Forms.Label
  Friend WithEvents pMatchingGrid As atcControls.atcGrid
  Friend WithEvents pSelectedGrid As atcControls.atcGrid
  Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
  Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuAttributes As System.Windows.Forms.MenuItem
  Friend WithEvents mnuSelect As System.Windows.Forms.MenuItem
  Friend WithEvents mnuAttributesAdd As System.Windows.Forms.MenuItem
  Friend WithEvents mnuAttributesRemove As System.Windows.Forms.MenuItem
  Friend WithEvents mnuAttributesMove As System.Windows.Forms.MenuItem
  Friend WithEvents mnuSelectAllMatching As System.Windows.Forms.MenuItem
  Friend WithEvents mnuSelectClear As System.Windows.Forms.MenuItem
  Friend WithEvents mnuSelectNoMatching As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileManage As System.Windows.Forms.MenuItem
  Friend WithEvents mnuAddData As System.Windows.Forms.MenuItem
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmSelectData))
    Me.groupTop = New System.Windows.Forms.GroupBox
    Me.pMatchingGrid = New atcControls.atcGrid
    Me.lblMatching = New System.Windows.Forms.Label
    Me.splitAboveMatching = New System.Windows.Forms.Splitter
    Me.panelCriteria = New System.Windows.Forms.Panel
    Me.pnlButtons = New System.Windows.Forms.Panel
    Me.btnCancel = New System.Windows.Forms.Button
    Me.btnOk = New System.Windows.Forms.Button
    Me.splitAboveSelected = New System.Windows.Forms.Splitter
    Me.groupSelected = New System.Windows.Forms.GroupBox
    Me.pSelectedGrid = New atcControls.atcGrid
    Me.MainMenu1 = New System.Windows.Forms.MainMenu
    Me.MenuItem1 = New System.Windows.Forms.MenuItem
    Me.mnuAddData = New System.Windows.Forms.MenuItem
    Me.mnuFileManage = New System.Windows.Forms.MenuItem
    Me.mnuAttributes = New System.Windows.Forms.MenuItem
    Me.mnuAttributesAdd = New System.Windows.Forms.MenuItem
    Me.mnuAttributesRemove = New System.Windows.Forms.MenuItem
    Me.mnuAttributesMove = New System.Windows.Forms.MenuItem
    Me.mnuSelect = New System.Windows.Forms.MenuItem
    Me.mnuSelectAllMatching = New System.Windows.Forms.MenuItem
    Me.mnuSelectNoMatching = New System.Windows.Forms.MenuItem
    Me.mnuSelectClear = New System.Windows.Forms.MenuItem
    Me.groupTop.SuspendLayout()
    Me.pnlButtons.SuspendLayout()
    Me.groupSelected.SuspendLayout()
    Me.SuspendLayout()
    '
    'groupTop
    '
    Me.groupTop.Controls.Add(Me.pMatchingGrid)
    Me.groupTop.Controls.Add(Me.lblMatching)
    Me.groupTop.Controls.Add(Me.splitAboveMatching)
    Me.groupTop.Controls.Add(Me.panelCriteria)
    Me.groupTop.Dock = System.Windows.Forms.DockStyle.Top
    Me.groupTop.Location = New System.Drawing.Point(0, 0)
    Me.groupTop.Name = "groupTop"
    Me.groupTop.Size = New System.Drawing.Size(528, 352)
    Me.groupTop.TabIndex = 10
    Me.groupTop.TabStop = False
    Me.groupTop.Text = "Select Attribute Values to Filter Available Data"
    '
    'pMatchingGrid
    '
    Me.pMatchingGrid.Dock = System.Windows.Forms.DockStyle.Fill
    Me.pMatchingGrid.LineColor = System.Drawing.Color.Empty
    Me.pMatchingGrid.LineWidth = 0.0!
    Me.pMatchingGrid.Location = New System.Drawing.Point(3, 184)
    Me.pMatchingGrid.Name = "pMatchingGrid"
    Me.pMatchingGrid.Size = New System.Drawing.Size(522, 165)
    Me.pMatchingGrid.TabIndex = 15
    '
    'lblMatching
    '
    Me.lblMatching.Dock = System.Windows.Forms.DockStyle.Top
    Me.lblMatching.Location = New System.Drawing.Point(3, 168)
    Me.lblMatching.Name = "lblMatching"
    Me.lblMatching.Size = New System.Drawing.Size(522, 16)
    Me.lblMatching.TabIndex = 14
    Me.lblMatching.Text = "Matching Data (click to select)"
    '
    'splitAboveMatching
    '
    Me.splitAboveMatching.Dock = System.Windows.Forms.DockStyle.Top
    Me.splitAboveMatching.Location = New System.Drawing.Point(3, 160)
    Me.splitAboveMatching.Name = "splitAboveMatching"
    Me.splitAboveMatching.Size = New System.Drawing.Size(522, 8)
    Me.splitAboveMatching.TabIndex = 12
    Me.splitAboveMatching.TabStop = False
    '
    'panelCriteria
    '
    Me.panelCriteria.Dock = System.Windows.Forms.DockStyle.Top
    Me.panelCriteria.Location = New System.Drawing.Point(3, 16)
    Me.panelCriteria.Name = "panelCriteria"
    Me.panelCriteria.Size = New System.Drawing.Size(522, 144)
    Me.panelCriteria.TabIndex = 11
    '
    'pnlButtons
    '
    Me.pnlButtons.Controls.Add(Me.btnCancel)
    Me.pnlButtons.Controls.Add(Me.btnOk)
    Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
    Me.pnlButtons.Location = New System.Drawing.Point(0, 485)
    Me.pnlButtons.Name = "pnlButtons"
    Me.pnlButtons.Size = New System.Drawing.Size(528, 40)
    Me.pnlButtons.TabIndex = 12
    '
    'btnCancel
    '
    Me.btnCancel.Location = New System.Drawing.Point(104, 8)
    Me.btnCancel.Name = "btnCancel"
    Me.btnCancel.Size = New System.Drawing.Size(80, 24)
    Me.btnCancel.TabIndex = 4
    Me.btnCancel.Text = "Cancel"
    '
    'btnOk
    '
    Me.btnOk.Location = New System.Drawing.Point(8, 8)
    Me.btnOk.Name = "btnOk"
    Me.btnOk.Size = New System.Drawing.Size(80, 24)
    Me.btnOk.TabIndex = 3
    Me.btnOk.Text = "Ok"
    '
    'splitAboveSelected
    '
    Me.splitAboveSelected.Dock = System.Windows.Forms.DockStyle.Top
    Me.splitAboveSelected.Location = New System.Drawing.Point(0, 352)
    Me.splitAboveSelected.Name = "splitAboveSelected"
    Me.splitAboveSelected.Size = New System.Drawing.Size(528, 8)
    Me.splitAboveSelected.TabIndex = 11
    Me.splitAboveSelected.TabStop = False
    '
    'groupSelected
    '
    Me.groupSelected.Controls.Add(Me.pSelectedGrid)
    Me.groupSelected.Dock = System.Windows.Forms.DockStyle.Fill
    Me.groupSelected.Location = New System.Drawing.Point(0, 360)
    Me.groupSelected.Name = "groupSelected"
    Me.groupSelected.Size = New System.Drawing.Size(528, 125)
    Me.groupSelected.TabIndex = 14
    Me.groupSelected.TabStop = False
    Me.groupSelected.Text = "Selected Data"
    '
    'pSelectedGrid
    '
    Me.pSelectedGrid.Dock = System.Windows.Forms.DockStyle.Fill
    Me.pSelectedGrid.LineColor = System.Drawing.Color.Empty
    Me.pSelectedGrid.LineWidth = 0.0!
    Me.pSelectedGrid.Location = New System.Drawing.Point(3, 16)
    Me.pSelectedGrid.Name = "pSelectedGrid"
    Me.pSelectedGrid.Size = New System.Drawing.Size(522, 106)
    Me.pSelectedGrid.TabIndex = 0
    '
    'MainMenu1
    '
    Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.mnuAttributes, Me.mnuSelect})
    '
    'MenuItem1
    '
    Me.MenuItem1.Index = 0
    Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuAddData, Me.mnuFileManage})
    Me.MenuItem1.Text = "&File"
    '
    'mnuAddData
    '
    Me.mnuAddData.Index = 0
    Me.mnuAddData.Text = "&Add Data"
    '
    'mnuFileManage
    '
    Me.mnuFileManage.Index = 1
    Me.mnuFileManage.Text = "&Manage Data Sources"
    '
    'mnuAttributes
    '
    Me.mnuAttributes.Index = 1
    Me.mnuAttributes.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuAttributesAdd, Me.mnuAttributesRemove, Me.mnuAttributesMove})
    Me.mnuAttributes.Text = "&Attributes"
    '
    'mnuAttributesAdd
    '
    Me.mnuAttributesAdd.Index = 0
    Me.mnuAttributesAdd.Text = "A&dd"
    '
    'mnuAttributesRemove
    '
    Me.mnuAttributesRemove.Index = 1
    Me.mnuAttributesRemove.Text = "&Remove"
    '
    'mnuAttributesMove
    '
    Me.mnuAttributesMove.Index = 2
    Me.mnuAttributesMove.Text = "&Move"
    '
    'mnuSelect
    '
    Me.mnuSelect.Index = 2
    Me.mnuSelect.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuSelectAllMatching, Me.mnuSelectNoMatching, Me.mnuSelectClear})
    Me.mnuSelect.Text = "&Select"
    '
    'mnuSelectAllMatching
    '
    Me.mnuSelectAllMatching.Index = 0
    Me.mnuSelectAllMatching.Text = "Select &Matching"
    '
    'mnuSelectNoMatching
    '
    Me.mnuSelectNoMatching.Index = 1
    Me.mnuSelectNoMatching.Text = "&Un-select Matching"
    '
    'mnuSelectClear
    '
    Me.mnuSelectClear.Index = 2
    Me.mnuSelectClear.Text = "&Clear"
    '
    'frmSelectData
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(528, 525)
    Me.Controls.Add(Me.groupSelected)
    Me.Controls.Add(Me.splitAboveSelected)
    Me.Controls.Add(Me.pnlButtons)
    Me.Controls.Add(Me.groupTop)
    Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
    Me.Menu = Me.MainMenu1
    Me.Name = "frmSelectData"
    Me.Text = "Select Data"
    Me.groupTop.ResumeLayout(False)
    Me.pnlButtons.ResumeLayout(False)
    Me.groupSelected.ResumeLayout(False)
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private Const PADDING As Integer = 5
  'Private Const REMOVE_VALUE = "~Remove~"
  Private Const NOTHING_VALUE = "~Missing~"

  Private pcboCriteria() As Windows.Forms.ComboBox
  Private plstCriteria() As Windows.Forms.ListBox
  Private pCriteriaFraction() As Single

  Private WithEvents pDataManager As atcDataManager

  Private pMatchingTS As atcDataGroup
  Private pSelectedTS As atcDataGroup
  Private pSaveGroup As atcDataGroup = Nothing

  Private pMatchingSource As GridSource
  Private pSelectedSource As GridSource

  Private pInitializing As Boolean
  Private pSelectedOK As Boolean

  Private pTotalTS As Integer

  Public Function AskUser(ByVal aDataManager As atcDataManager, Optional ByVal aGroup As atcDataGroup = Nothing, Optional ByVal aModal As Boolean = True) As atcDataGroup
    If aGroup Is Nothing Then
      pSelectedTS = New atcDataGroup
    Else
      pSaveGroup = aGroup.Clone
      pSelectedTS = aGroup
    End If

    pDataManager = aDataManager

    'If pDataManager.DataSources.Count = 1 Then
    '  pDataManager.UserManage()
    '  While pDataManager.DataSources.Count = 1
    '    Application.DoEvents()
    '  End While
    'End If

    pMatchingTS = New atcDataGroup
    pMatchingSource = New GridSource(pDataManager, pMatchingTS)
    pSelectedSource = New GridSource(pDataManager, pSelectedTS)

    pMatchingGrid.Initialize(pMatchingSource)
    pSelectedGrid.Initialize(pSelectedSource)

    Populate()
    If aModal Then
      Me.ShowDialog()
      If Not pSelectedOK Then 'User clicked Cancel or closed dialog
        pSelectedTS.ChangeTo(pSaveGroup)
      End If
    Else
      Me.Show()
    End If
    Return pSelectedTS
  End Function

  Private Sub Populate()
    pInitializing = True

    Try
      For iCriteria As Integer = pcboCriteria.GetUpperBound(0) To 0 Step -1
        RemoveCriteria(pcboCriteria(iCriteria), plstCriteria(iCriteria))
      Next
    Catch ex As Exception
      'first time through there is nothing to remove, error is normal
    End Try

    ReDim pcboCriteria(0)
    ReDim plstCriteria(0)
    ReDim pCriteriaFraction(0)

    For Each lAttribName As String In pDataManager.SelectionAttributes
      AddCriteria(lAttribName)
    Next

    PopulateMatching()
    pInitializing = False
    UpdatedCriteria()
  End Sub

  Private Sub PopulateCriteriaCombos()
    Dim i As Integer
    For i = 0 To pcboCriteria.GetUpperBound(0)
      pcboCriteria(i).Items.Clear()
      'If pcboCriteria.GetUpperBound(0) > 0 Then
      '  pcboCriteria(i).Items.Add(REMOVE_VALUE)
      'End If
    Next
    For Each source As atcDataSource In pDataManager.DataSources
      For Each ts As atcDataSet In source.DataSets
        For Each de As DictionaryEntry In ts.Attributes.GetAll
          If Not pcboCriteria(0).Items.Contains(de.Key) Then
            For i = 0 To pcboCriteria.GetUpperBound(0)
              pcboCriteria(i).Items.Add(de.Key)
            Next
          End If
        Next
      Next
    Next
  End Sub

  Private Sub PopulateCriteriaList(ByVal aAttributeName As String, ByVal aList As Windows.Forms.ListBox)
    aList.Items.Clear()
    For Each source As atcDataSource In pDataManager.DataSources
      For Each ts As atcDataSet In source.DataSets
        Dim val As Object = ts.Attributes.GetValue(aAttributeName, Nothing)
        If val Is Nothing Then val = NOTHING_VALUE
        If Not aList.Items.Contains(val) Then
          aList.Items.Add(val)
        End If
      Next
    Next
  End Sub

  Private Sub PopulateMatching()
    Dim iLastCriteria As Integer = pcboCriteria.GetUpperBound(0)
    pMatchingTS.Clear()
    pTotalTS = 0
    For Each source As atcDataSource In pDataManager.DataSources
      For Each ts As atcDataSet In source.DataSets
        pTotalTS += 1
        For iCriteria As Integer = 0 To iLastCriteria
          Dim attrName As String = pcboCriteria(iCriteria).SelectedItem
          If Not attrName Is Nothing Then
            Dim selectedValues As Windows.Forms.ListBox.SelectedObjectCollection = plstCriteria(iCriteria).SelectedItems
            If selectedValues.Count > 0 Then 'none selected = all selected
              Dim attrValue As Object = ts.Attributes.GetValue(attrName, Nothing)
              If attrValue Is Nothing Then attrValue = NOTHING_VALUE
              If Not selectedValues.Contains(attrValue) Then 'Does not match this criteria
                GoTo NextTS
              End If
            End If
          End If
        Next
        'Matched all criteria, add to matching table
        pMatchingTS.Add(ts)
NextTS:
      Next
    Next
    lblMatching.Text = "Matching Data (" & pMatchingTS.Count & " of " & pTotalTS & ")"
    pMatchingGrid.Refresh()
  End Sub

  Private Function GetIndex(ByVal aName As String) As Integer
    Return CInt(Mid(aName, InStr(aName, "#") + 1))
  End Function

  Private Sub cboCriteria_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    If Not sender.SelectedItem Is Nothing Then
      'If sender.SelectedItem = REMOVE_VALUE Then
      '  If plstCriteria.GetUpperBound(0) > 0 Then
      '    RemoveCriteria(sender, plstCriteria(GetIndex(sender.name)))
      '  End If
      'Else
      PopulateCriteriaList(sender.SelectedItem, plstCriteria(GetIndex(sender.name)))
      UpdatedCriteria()
      'End If
    End If
  End Sub

  Private Sub lstCriteria_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    UpdatedCriteria()
  End Sub

  Private Sub UpdatedCriteria()
    If Not pInitializing Then
      Dim mnu As MenuItem
      Dim iLastCriteria As Integer = pcboCriteria.GetUpperBound(0)

      UpdateManagerSelectionAttributes()
      PopulateMatching()
      RefreshSelected()

      For Each mnu In mnuAttributesRemove.MenuItems
        RemoveHandler mnu.Click, AddressOf mnuRemove_Click
      Next
      For Each mnu In mnuAttributesMove.MenuItems
        RemoveHandler mnu.Click, AddressOf mnuMove_Click
      Next

      mnuAttributesRemove.MenuItems.Clear()
      mnuAttributesMove.MenuItems.Clear()

      If iLastCriteria > 0 Then 'Only allow moving/removing if more than one exists
        For iCriteria As Integer = 0 To iLastCriteria
          mnu = mnuAttributesRemove.MenuItems.Add("&" & iCriteria + 1 & " " & pcboCriteria(iCriteria).SelectedItem)
          AddHandler mnu.Click, AddressOf mnuRemove_Click
          mnu = mnuAttributesMove.MenuItems.Add("&" & iCriteria + 1 & " " & pcboCriteria(iCriteria).SelectedItem)
          AddHandler mnu.Click, AddressOf mnuMove_Click
        Next
      End If

    End If
  End Sub

  Private Sub RemoveCriteria(ByVal cbo As Windows.Forms.ComboBox, ByVal lst As Windows.Forms.ListBox)
    Dim iRemoving As Integer = GetIndex(cbo.Name)
    Dim newLastCriteria As Integer = pcboCriteria.GetUpperBound(0) - 1
    Dim OldToNew As Single = 1 / (1 - pCriteriaFraction(iRemoving))
    Dim mnu As MenuItem
    RemoveHandler cbo.SelectedValueChanged, AddressOf cboCriteria_SelectedIndexChanged
    RemoveHandler lst.SelectedValueChanged, AddressOf lstCriteria_SelectedIndexChanged
    panelCriteria.Controls.Remove(cbo)
    panelCriteria.Controls.Remove(lst)

    For iMoving As Integer = iRemoving To pcboCriteria.GetUpperBound(0) - 1
      pcboCriteria(iMoving) = pcboCriteria(iMoving + 1)
      plstCriteria(iMoving) = plstCriteria(iMoving + 1)
      pcboCriteria(iMoving).Name = "cboCriteria#" & iMoving
      plstCriteria(iMoving).Name = "lstCriteria#" & iMoving
      pCriteriaFraction(iMoving) = pCriteriaFraction(iMoving + 1)
    Next

    ReDim Preserve pcboCriteria(newLastCriteria)
    ReDim Preserve plstCriteria(newLastCriteria)
    ReDim Preserve pCriteriaFraction(newLastCriteria)

    'Expand remaining criteria proportionally to fill space
    For iScanCriteria As Integer = 0 To newLastCriteria
      pCriteriaFraction(iScanCriteria) *= OldToNew
    Next

    'If pcboCriteria.GetUpperBound(0) = 0 Then
    '  pcboCriteria(0).Items.Remove(REMOVE_VALUE)
    'End If

    SizeCriteria()
    UpdatedCriteria()
  End Sub

  Private Sub AddCriteria(Optional ByVal aText As String = "")
    Dim iCriteria As Integer = pcboCriteria.GetUpperBound(0)

    If Not pcboCriteria(iCriteria) Is Nothing Then 'If we already populated this index, move to next one
      iCriteria += 1                               'This happens every time except for the first one
      ReDim Preserve pcboCriteria(iCriteria)
      ReDim Preserve plstCriteria(iCriteria)
      ReDim Preserve pCriteriaFraction(iCriteria)
    End If

    Dim fractionInUse As Single = 0
    For iScanCriteria As Integer = 0 To iCriteria - 1
      fractionInUse += pCriteriaFraction(iScanCriteria)
    Next

    Dim newEqualPortion As Single = 1 / (iCriteria + 1)
    Dim totalShrinkingNeeded As Single = fractionInUse + newEqualPortion - 1

    'Default to give new one an equal portion of the width
    pCriteriaFraction(iCriteria) = newEqualPortion

    If totalShrinkingNeeded > 0 Then 'Not enough extra unused space
      'Shrink existing criteria proportionally to fit the new one in
      For iScanCriteria As Integer = 0 To iCriteria - 1
        pCriteriaFraction(iScanCriteria) *= (1 - totalShrinkingNeeded)
      Next
    End If

    pcboCriteria(iCriteria) = New Windows.Forms.ComboBox
    plstCriteria(iCriteria) = New Windows.Forms.ListBox

    panelCriteria.Controls.Add(pcboCriteria(iCriteria))
    panelCriteria.Controls.Add(plstCriteria(iCriteria))

    AddHandler pcboCriteria(iCriteria).SelectedValueChanged, AddressOf cboCriteria_SelectedIndexChanged
    AddHandler plstCriteria(iCriteria).SelectedValueChanged, AddressOf lstCriteria_SelectedIndexChanged

    With pcboCriteria(iCriteria)
      .Name = "cboCriteria#" & iCriteria
      .DropDownStyle = Windows.Forms.ComboBoxStyle.DropDownList
      .MaxDropDownItems = 40
      .Sorted = True
    End With

    With plstCriteria(iCriteria)
      .Name = "lstCriteria#" & iCriteria
      .IntegralHeight = False
      .SelectionMode = Windows.Forms.SelectionMode.MultiSimple
      .Sorted = True
      '.Dock = Windows.Forms.DockStyle.Left
    End With

    If iCriteria = 0 Then
      PopulateCriteriaCombos()
    Else 'populate from first combo box
      'If Not pcboCriteria(0).Items.Contains(REMOVE_VALUE) Then
      '  pcboCriteria(0).Items.Add(REMOVE_VALUE)
      'End If

      For iItem As Integer = 0 To pcboCriteria(0).Items.Count - 1
        pcboCriteria(iCriteria).Items.Add(pcboCriteria(0).Items.Item(iItem))
      Next
    End If
    If aText.Length > 0 Then
      pcboCriteria(iCriteria).Text = aText
    Else 'Find next criteria that is not yet in use
      For Each curName As String In pcboCriteria(iCriteria).Items
        'If curName <> REMOVE_VALUE Then
        For iOtherCriteria As Integer = 0 To iCriteria - 1
          If curName.Equals(pcboCriteria(iOtherCriteria).SelectedItem) Then GoTo NextName
        Next
        pcboCriteria(iCriteria).Text = curName
        Exit For
        'End If
NextName:
      Next
    End If

    UpdatedCriteria()
    SizeCriteria()
  End Sub

  Private Sub panelCriteria_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles panelCriteria.SizeChanged
    SizeCriteria()
  End Sub

  Private Sub ResizeOneCriteria(ByVal aCriteria As Integer, ByVal aWidth As Integer)
    Dim iLastCriteria As Integer = pcboCriteria.GetUpperBound(0)
    pcboCriteria(aCriteria).Width = aWidth - PADDING
    pCriteriaFraction(aCriteria) = pcboCriteria(aCriteria).Width / (panelCriteria.Width - PADDING)
    plstCriteria(aCriteria).Width = pcboCriteria(aCriteria).Width
    While aCriteria < iLastCriteria
      aCriteria += 1
      pcboCriteria(aCriteria).Left = pcboCriteria(aCriteria - 1).Left + pcboCriteria(aCriteria - 1).Width + PADDING
      plstCriteria(aCriteria).Left = pcboCriteria(aCriteria).Left
    End While

    'Fit rightmost criteria to fill remaining space
    Dim availableWidth As Integer = panelCriteria.Width - PADDING * 2
    If pcboCriteria(iLastCriteria).Left < availableWidth Then
      pcboCriteria(iLastCriteria).Width = availableWidth - pcboCriteria(iLastCriteria).Left
      plstCriteria(iLastCriteria).Width = pcboCriteria(iLastCriteria).Width
    End If
  End Sub

  Private Sub SizeCriteria()
    If Not pcboCriteria Is Nothing Then
      Dim iLastCriteria As Integer = pcboCriteria.GetUpperBound(0)
      If iLastCriteria >= 0 Then
        Dim eachCriteriaPortion() As Integer
        Dim availableWidth As Integer = panelCriteria.Width - PADDING
        'Dim perCriteriaWidth As Integer = (panelCriteria.Width - PADDING) / (iLastCriteria + 1)
        Dim curLeft As Integer = PADDING

        pMatchingGrid.ColumnWidth(0) = 0
        pSelectedGrid.ColumnWidth(0) = 0

        For iCriteria As Integer = 0 To iLastCriteria
          pcboCriteria(iCriteria).Top = PADDING
          pcboCriteria(iCriteria).Left = curLeft
          If iCriteria = iLastCriteria AndAlso curLeft < availableWidth Then 'Fit rightmost criteria to fill remaining space
            pcboCriteria(iCriteria).Width = availableWidth - curLeft
          Else
            pcboCriteria(iCriteria).Width = availableWidth * pCriteriaFraction(iCriteria)
          End If
          If pcboCriteria(iCriteria).Width > PADDING Then pcboCriteria(iCriteria).Width -= PADDING

          plstCriteria(iCriteria).Top = pcboCriteria(iCriteria).Top + pcboCriteria(iCriteria).Height + PADDING
          plstCriteria(iCriteria).Left = curLeft
          plstCriteria(iCriteria).Width = pcboCriteria(iCriteria).Width
          plstCriteria(iCriteria).Height = panelCriteria.Height - plstCriteria(iCriteria).Top - PADDING

          curLeft = pcboCriteria(iCriteria).Left + pcboCriteria(iCriteria).Width + PADDING

          pMatchingGrid.ColumnWidth(iCriteria + 1) = pcboCriteria(iCriteria).Width + PADDING
          pSelectedGrid.ColumnWidth(iCriteria + 1) = pMatchingGrid.ColumnWidth(iCriteria + 1)
        Next
        pMatchingGrid.Refresh()
        pSelectedGrid.Refresh()
      End If
    End If
  End Sub

  Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
    pSelectedOK = True
    Me.Close()
  End Sub

  Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
    pSelectedOK = False
    pSelectedTS.ChangeTo(pSaveGroup)
    Me.Close()
  End Sub

  'Update SelectionAttributes from current set of pcboCriteria
  Private Sub UpdateManagerSelectionAttributes()
    Dim curAttributes As New ArrayList
    For iCriteria As Integer = 0 To pcboCriteria.GetUpperBound(0)
      Dim attrName As String = pcboCriteria(iCriteria).SelectedItem
      If Not attrName Is Nothing Then
        curAttributes.Add(attrName)
      End If
    Next
    If curAttributes.Count > 0 Then
      pDataManager.SelectionAttributes.Clear()
      pDataManager.SelectionAttributes.AddRange(curAttributes)
    End If
  End Sub

  Private Sub pMatchingGrid_MouseDownCell(ByVal aRow As Integer, ByVal aColumn As Integer) Handles pMatchingGrid.MouseDownCell
    If IsNumeric(pMatchingSource.CellValue(aRow, 0)) Then 'clicked a row containing a serial number
      Dim lSerial As Integer = CInt(pMatchingSource.CellValue(aRow, 0)) 'Serial number in clicked row
      Dim iTS As Integer = pSelectedTS.IndexOfSerial(lSerial)
      If iTS >= 0 Then 'Already selected, unselect
        pSelectedTS.Remove(iTS)
      Else 'Not already selected, select it now
        iTS = pMatchingTS.IndexOfSerial(lSerial)
        If iTS >= 0 Then 'Found matching serial number in pMatchingTS
          Dim selTS As atcData.atcDataSet = pMatchingTS(iTS)
          pSelectedTS.Add(selTS)
        End If
      End If
    End If
    RefreshSelected()
  End Sub

  Private Sub RefreshSelected()
    pSelectedGrid.Refresh()
    groupSelected.Text = "Selected Data (" & pSelectedTS.Count & " of " & pTotalTS & ")"
  End Sub

  Private Sub pSelectedGrid_MouseDownCell(ByVal aRow As Integer, ByVal aColumn As Integer) Handles pSelectedGrid.MouseDownCell
    If IsNumeric(pSelectedSource.CellValue(aRow, 0)) Then 'clicked a row containing a serial number
      Dim lSerial As Integer = CInt(pSelectedSource.CellValue(aRow, 0)) 'Serial number in row to be removed
      Dim iTS As Integer = pSelectedTS.IndexOfSerial(lSerial)
      If iTS >= 0 Then 'Found matching serial number in pSelectedTS
        pSelectedTS.Remove(iTS)
        RefreshSelected()
      Else
        'TODO: should never reach this line
      End If
    End If
  End Sub

  Private Sub pDataManager_OpenedData(ByVal aDataSource As atcDataSource) Handles pDataManager.OpenedData
    Populate()
  End Sub

  Private Sub pMatchingGrid_UserResizedColumn(ByVal aColumn As Integer, ByVal aWidth As Integer) Handles pMatchingGrid.UserResizedColumn
    pSelectedGrid.ColumnWidth(aColumn) = aWidth
    pSelectedGrid.Refresh()
    ResizeOneCriteria(aColumn - 1, aWidth)
  End Sub

  Private Sub pSelectedGrid_UserResizedColumn(ByVal aColumn As Integer, ByVal aWidth As Integer) Handles pSelectedGrid.UserResizedColumn
    pMatchingGrid.ColumnWidth(aColumn) = aWidth
    pMatchingGrid.Refresh()
    ResizeOneCriteria(aColumn - 1, aWidth)
  End Sub

  Private Sub mnuAttributesAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAttributesAdd.Click
    AddCriteria()
  End Sub

  Private Sub mnuSelectClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectClear.Click
    pSelectedTS.Clear()
    RefreshSelected()
  End Sub

  Private Sub mnuSelectAllMatching_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectAllMatching.Click
    For Each ts As atcDataSet In pMatchingTS
      If Not pSelectedTS.Contains(ts) Then pSelectedTS.Add(ts)
    Next
    RefreshSelected()
  End Sub

  Private Sub mnuSelectNoMatching_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSelectNoMatching.Click
    For Each ts As atcDataSet In pMatchingTS
      If pSelectedTS.Contains(ts) Then pSelectedTS.Remove(ts)
    Next
    RefreshSelected()
  End Sub

  Private Sub mnuFileManage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileManage.Click
    pDataManager.UserManage() ' .OpenData("")
  End Sub

  Private Sub mnuRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    Dim mnu As MenuItem = sender
    Dim index As Integer = mnu.Index
    RemoveCriteria(pcboCriteria(index), plstCriteria(index))
  End Sub

  Private Sub mnuMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub mnuAddData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAddData.Click
    pDataManager.UserOpenDataSource(Nothing, pSelectedTS)
  End Sub
End Class

Friend Class GridSource
  Inherits atcControls.atcGridSource

  ' 0 to label the columns in row 0
  '-1 to not label columns
  Private Const LabelRow As Integer = -1

  Private pDataManager As atcDataManager
  Private pDataGroup As atcDataGroup

  Sub New(ByVal aDataManager As atcData.atcDataManager, _
          ByVal aDataGroup As atcData.atcDataGroup)
    pDataManager = aDataManager
    pDataGroup = aDataGroup
  End Sub

  Public Overrides Property Columns() As Integer
    Get
      Return pDataManager.SelectionAttributes.Count() + 1
    End Get
    Set(ByVal Value As Integer)
    End Set
  End Property

  Public Overrides Property Rows() As Integer
    Get
      Return pDataGroup.Count + LabelRow + 1
    End Get
    Set(ByVal Value As Integer)
    End Set
  End Property

  Public Overrides Property CellValue(ByVal aRow As Integer, ByVal aColumn As Integer) As String
    Get
      If aRow = LabelRow Then
        If aColumn = 0 Then
          Return ""
        Else
          Return pDataManager.SelectionAttributes(aColumn - 1)
        End If
      ElseIf aColumn = 0 Then
        Return pDataGroup(aRow - (LabelRow + 1)).Serial()
      Else
        Return pDataGroup(aRow - (LabelRow + 1)).Attributes.GetValue(pDataManager.SelectionAttributes(aColumn - 1))
      End If
    End Get
    Set(ByVal Value As String)
    End Set
  End Property

  Public Overrides Property Alignment(ByVal aRow As Integer, ByVal aColumn As Integer) As atcControls.atcAlignment
    Get
      Return atcControls.atcAlignment.HAlignLeft
    End Get
    Set(ByVal Value As atcControls.atcAlignment)
    End Set
  End Property
End Class