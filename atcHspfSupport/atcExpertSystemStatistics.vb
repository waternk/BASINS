Imports atcUtility
Imports atcData
Imports MapWinUtility
Imports Microsoft.VisualBasic

Public Module ExpertSystemStatistics
    Private Const pNSteps As Integer = 500
    Private Const pNStatGroups As Integer = 6
    Private Const pNStats As Integer = 10
    Private Const pNErrorTerms As Integer = 11
    Private Const pConvert As Double = 24.0# * 3600.0# * 12.0# / 43560.0#

    Dim pSJdate As Double, pEJdate As Double
    Dim pErrorTermNames(11) As String
    Dim pErrorTerms(,) As Double
    Dim pStatNames(10) As String
    Dim pStats(,,) As Double
    Dim pExpertSystem As HspfSupport.ExpertSystem

    Public Function Report(ByVal aUci As atcUCI.HspfUci, ByVal aDataSource As atcDataSource) As String
        pErrorTermNames(1) = "Error in total volume (%)"
        pErrorTermNames(2) = "Error in low-flow recession"
        pErrorTermNames(3) = "Error in 50% lowest flows (%)"
        pErrorTermNames(4) = "Error in 10% highest flows (%)"
        pErrorTermNames(5) = "Error in storm volumes (%)"
        pErrorTermNames(6) = "Ratio of interflow to surface runoff (in/in)"
        pErrorTermNames(7) = "Seasonal volume error (%)"
        pErrorTermNames(8) = "Summer storm volume error (%)"
        pErrorTermNames(9) = "Multiplier on third and fourth error terms"
        pErrorTermNames(10) = "Percent of flows to use in low-flow recession error"
        pErrorTermNames(11) = "Average storm peak flow error (%)"

        pStatNames(1) = "total (inches)"
        pStatNames(2) = "50% low (inches)"
        pStatNames(3) = "10% high (inches)"
        pStatNames(4) = "storm volume (inches)"
        pStatNames(5) = "average storm peak (cfs)"
        pStatNames(6) = "baseflow recession rate"
        pStatNames(7) = "summer volume (inches)"
        pStatNames(8) = "winter volume (inches)"
        pStatNames(9) = "summer storms (inches)"
        pStatNames(10) = "winter storms (inches)"

        pExpertSystem = New HspfSupport.ExpertSystem(FilenameOnly(aUci.Name))
        pSJdate = aUci.GlobalBlock.SDateJ
        pEJdate = aUci.GlobalBlock.EdateJ

        CalcStats(aDataSource)
        Dim lStr As String = CalcErrorTerms(aUci)
        Return lStr
    End Function

    Private Sub CalcStats(ByVal aDataSource As atcDataSource)
        'lStatGroup - Group number of pStats desired from this call
        '         1-sim runoff, 2-obs runoff, 3-sim stm sur runoff,
        '         4-sim stm interflow, 5-tot pot et, 6-act et
        'STAT   - Calculated statistics
        '         1-total, 2-50%low, 3-10%high, 4-storm vol, 5-storm peak,
        '         6-recess, 7-sum vol, 8-win vol, 9-sum strm, 10-win strm

        ReDim pStats(pNStats, pNStatGroups, pExpertSystem.NSites)

        'get number of values
        Dim lTimeStep As Integer, lTimeUnit As Integer, lNVals As Integer
        lTimeStep = 1
        lTimeUnit = 4 'day
        lNVals = timdifJ(pSJdate, pEJdate, lTimeUnit, lTimeStep)

        For lSiteIndex As Integer = 1 To pExpertSystem.NSites
            For lStatGroup As Integer = 1 To pNStatGroups
                'set Stats to undefined for this group
                ZipR(pNStats, Double.NaN, pStats, lStatGroup, lSiteIndex)

                Dim lDSN As Integer
                Select Case lStatGroup 'get the correct dsn
                    Case 1 : lDSN = pExpertSystem.SiteDsn(0, lSiteIndex)
                    Case 2 : lDSN = pExpertSystem.SiteDsn(1, lSiteIndex)
                    Case 3 : lDSN = pExpertSystem.SiteDsn(2, lSiteIndex)
                    Case 4 : lDSN = pExpertSystem.SiteDsn(3, lSiteIndex)
                    Case 5 : lDSN = pExpertSystem.SiteDsn(6, lSiteIndex)
                    Case 6 : lDSN = pExpertSystem.SiteDsn(7, lSiteIndex)
                End Select

                'Get data - daily values and max values as necessary
                Dim lTSer As atcTimeseries = aDataSource.DataSets(aDataSource.DataSets.IndexFromKey(lDSN))
                Dim lNewTSer As atcTimeseries = SubsetByDate(lTSer, pSJdate, pEJdate, Nothing)
                lTSer = Nothing

                Dim lDailyTSer As atcTimeseries
                If lStatGroup = 2 Then 'observed flow in cfs, want average
                    lDailyTSer = Aggregate(lNewTSer, atcTimeUnit.TUDay, 1, atcTran.TranAverSame)
                Else 'want total values
                    lDailyTSer = Aggregate(lNewTSer, atcTimeUnit.TUDay, 1, atcTran.TranSumDiv)
                End If
                lNewTSer.Clear()
                lNewTSer.Dates.Clear()
                lNewTSer = Nothing

                Dim lValues() As Double = lDailyTSer.Values
                'check to make sure we got values
                Dim lDataProblem As Boolean = False
                If lDailyTSer.Values.Length = 0 Then
                    lDataProblem = True
                End If

                If lDataProblem Then  'if we weren't able to retrieve the data set
                    'set Stats to undefined
                    ZipR(pNStats, Double.NaN, pStats, lStatGroup, lSiteIndex)
                    Logger.Msg("Unable to retrieve DSN " & lDSN & vbCrLf & _
                               "from the file " & aDataSource.Name, "Bad Data Set")
                Else  'generate statistics
                    'total volume always needed; initialize total volume
                    pStats(1, lStatGroup, lSiteIndex) = 0.0#
                    For i As Integer = 1 To lNVals 'increment total volume
                        pStats(1, lStatGroup, lSiteIndex) += lValues(i)
                    Next i
                    'others?
                    If (lStatGroup = 1 Or lStatGroup = 2) Then  'full range of pStats desired
                        'summer volume
                        pStats(7, lStatGroup, lSiteIndex) = 0.0#
                        'winter volume
                        pStats(8, lStatGroup, lSiteIndex) = 0.0#

                        Dim lMaxVal As Double = 1000000.0#
                        Dim lMinVal As Double = 0.0001
                        Dim lArLgFg As Integer = 2
                        'use a box sort algorithm as an approximation for later
                        'totaling, putting values greater than upper limit in
                        'highest ranking box (box #1) and values less than lower
                        'limit in lowest ranking box (box #pNSteps).
                        Dim lBoxSum(pNSteps) As Double
                        Dim lBoxCnt(pNSteps) As Integer
                        BoxSort(lMaxVal, lMinVal, pNSteps, lNVals, lValues, lArLgFg, _
                                lBoxCnt, lBoxSum)

                        'total of lowest 50%
                        Dim lnV50 As Integer, lnV90 As Integer
                        lnV50 = lNVals / 2
                        If (lnV50 < 1) Then
                            'need at least one value
                            lnV50 = 1
                        End If
                        'find highest 50 percent ...
                        Dim lSum As Double = BoxAdd(lnV50, lBoxCnt, lBoxSum)

                        'then subtract from total to get lowest 50 percent
                        pStats(2, lStatGroup, lSiteIndex) = pStats(1, lStatGroup, lSiteIndex) - lSum

                        'total of highest 10%
                        lnV90 = lNVals * 0.1
                        If (lnV90 < 1) Then 'need at least one value
                            lnV90 = 1
                        End If
                        pStats(3, lStatGroup, lSiteIndex) = BoxAdd(lnV90, lBoxCnt, lBoxSum)

                        Dim lTmpDate(5) As Integer
                        J2Date(pSJdate, lTmpDate)
                        For i As Integer = 1 To lNVals
                            If (lTmpDate(1) = 12 Or lTmpDate(1) = 1 Or lTmpDate(1) = 2) Then
                                'in the winter
                                pStats(8, lStatGroup, lSiteIndex) += lValues(i)
                            ElseIf (lTmpDate(1) = 6 Or lTmpDate(1) = 7 Or lTmpDate(1) = 8) Then
                                'in the summer
                                pStats(7, lStatGroup, lSiteIndex) += lValues(i)
                            End If
                            TIMADD(lTmpDate, lTimeUnit, lTimeStep, lTimeStep, lTmpDate)
                        Next i
                    End If

                    If (lStatGroup >= 1 And lStatGroup <= 4) Then  'calc storm info
                        pStats(4, lStatGroup, lSiteIndex) = 0.0# 'initialize storm volume
                        pStats(5, lStatGroup, lSiteIndex) = 0.0# 'storm peaks
                        pStats(9, lStatGroup, lSiteIndex) = 0.0# 'summer storms
                        pStats(10, lStatGroup, lSiteIndex) = 0.0# 'winter storms
                        If (pExpertSystem.NStorms > 0) Then 'storms are available, loop thru them
                            For lStormIndex As Integer = 1 To pExpertSystem.NStorms
                                If pExpertSystem.StormSJDate(lStormIndex) >= pSJdate And _
                                   pExpertSystem.StormEJDate(lStormIndex) <= pEJdate Then 'storm within run span
                                    'TODO: this matches VB6Script results, needs to have indexes checked!
                                    Dim lN1 As Integer, lN2 As Integer
                                    lN1 = timdifJ(pSJdate, pExpertSystem.StormSJDate(lStormIndex), lTimeUnit, lTimeStep) + 1
                                    lN2 = timdifJ(pSJdate, pExpertSystem.StormEJDate(lStormIndex), lTimeUnit, lTimeStep)
                                    Dim lTmpDate(5) As Integer
                                    J2Date(pExpertSystem.StormSJDate(lStormIndex) - 1, lTmpDate)
                                    Dim lRtmp As Double = lDailyTSer.Values(lN1)
                                    For i As Integer = lN1 To lN2
                                        pStats(4, lStatGroup, lSiteIndex) += lValues(i)
                                        If (lDailyTSer.Values(i) > lRtmp) Then 'a new peak
                                            lRtmp = lDailyTSer.Values(i)
                                        End If
                                        If (lTmpDate(1) = 12 Or lTmpDate(1) = 1 Or lTmpDate(1) = 2) Then 'in the winter
                                            pStats(10, lStatGroup, lSiteIndex) += lValues(i)
                                        ElseIf (lTmpDate(1) = 6 Or lTmpDate(1) = 7 Or lTmpDate(1) = 8) Then 'in the summer
                                            pStats(9, lStatGroup, lSiteIndex) += lValues(i)
                                        End If
                                        TIMADD(lTmpDate, lTimeUnit, lTimeStep, lTimeStep, lTmpDate)
                                    Next i
                                    pStats(5, lStatGroup, lSiteIndex) += lRtmp
                                End If
                            Next lStormIndex
                        End If
                    End If

                    If (lStatGroup = 1 Or lStatGroup = 2) Then 'Change flows to recessions
                        'save first data value
                        Dim lSavDat As Double = lValues(1)
                        For lIndex As Integer = 2 To lNVals
                            Dim lRecession As Double
                            If (lSavDat > 0.0000000001) Then 'have some flow
                                lRecession = lValues(lIndex) / lSavDat
                            Else 'no flow
                                lRecession = Double.NaN
                            End If
                            lSavDat = lValues(lIndex)
                            lValues(lIndex - 1) = lRecession
                        Next lIndex
                        'adjust number of values to actual number processed
                        Dim lNValsProcessed As Integer = lNVals - 1

                        'sort the recessions
                        Dim lMaxVal As Double = 1.0#
                        Dim lMinVal As Double = 0.002#
                        Dim lArLgFg As Integer = 1
                        'use a box sort algorithm as an approximation for later
                        'totaling, putting values greater than upper limit in
                        'highest ranking box (box #1) and values less than lower
                        'limit in lowest ranking box (box #pNSteps).
                        Dim lRecCnt(pNSteps) As Integer
                        Dim lRecSum(pNSteps) As Double
                        BoxSort(lMaxVal, lMinVal, pNSteps, lNValsProcessed, lValues, lArLgFg, _
                                lRecCnt, lRecSum)
                        'Calc pStats
                        'new percent of time in base flow term
                        Dim lRtmp As Double = lNValsProcessed * pExpertSystem.ErrorCriteria(10) / 100
                        Dim lStmp As Double = lNValsProcessed - lRecCnt(1)
                        If (lStmp < lRtmp Or lRtmp < 1.0#) Then 'not enough values available
                            pStats(6, lStatGroup, lSiteIndex) = Double.NaN
                        Else
                            Dim lSum As Double = 0.0#
                            Dim lTotCnt As Double = 0.0#
                            Dim iIndex As Integer = 1
                            Do
                                'Loop through boxes starting with highest <=1.0,
                                'adding recession values until we have x percent of them
                                iIndex += 1
                                lSum += lRecSum(iIndex)
                                lTotCnt += lRecCnt(iIndex)
                                'have we added x percent of the values yet?
                            Loop While (lTotCnt < lRtmp)
                            'Compensate for inexact number of values by interpolating,
                            'subtract off a proportional amount of the box containing
                            'the cut-off value
                            Dim lSubVal As Double = (lTotCnt - lRtmp) * (lRecSum(iIndex) / lRecCnt(iIndex))
                            lSum -= lSubVal
                            'calculate average baseflow recession rate
                            pStats(6, lStatGroup, lSiteIndex) = lSum / lRtmp
                        End If
                    End If
                End If
                If lStatGroup = 1 Or lStatGroup = 3 Or lStatGroup = 4 Then 'take average over NStorms
                    pStats(5, lStatGroup, lSiteIndex) /= pExpertSystem.NStorms
                    'convert storm peak stat from acre-inch/day to cfs
                    pStats(5, lStatGroup, lSiteIndex) *= pExpertSystem.SiteArea(lSiteIndex) * 43560.0# / (12.0# * 24.0# * 3600.0#)
                ElseIf lStatGroup = 2 Then
                    For i As Integer = 1 To 10
                        If i < 5 Or i > 6 Then 'convert observed runoff values
                            pStats(i, lStatGroup, lSiteIndex) *= pConvert / pExpertSystem.SiteArea(lSiteIndex)
                        ElseIf i = 5 Then 'take average over NStorms
                            pStats(i, lStatGroup, lSiteIndex) /= pExpertSystem.NStorms
                        End If
                    Next i
                End If
                lDailyTSer.Clear()
                lDailyTSer.Dates.Clear()
                lDailyTSer = Nothing
            Next lStatGroup
        Next lSiteIndex
    End Sub

    Private Function CalcErrorTerms(ByVal auci As atcUCI.HspfUci) As String
        ReDim pErrorTerms(pNErrorTerms, pExpertSystem.NSites)

        For lSiteIndex As Integer = 1 To pExpertSystem.NSites
            'total volume error
            If (pStats(1, 2, lSiteIndex) > 0.0#) Then
                pErrorTerms(1, lSiteIndex) = 100.0# * ((pStats(1, 1, lSiteIndex) - pStats(1, 2, lSiteIndex)) _
                                                  / pStats(1, 2, lSiteIndex))
            Else
                pErrorTerms(1, lSiteIndex) = Double.NaN
            End If

            '     'total volume difference
            '      VOLDIF(lSiteIndex) = pStats(1, 1, lSiteIndex) - pStats(1, 2, lSiteIndex)
            '
            '     'unrealized potential evapotranspiration
            '      ETDIF(lSiteIndex) = pStats(1, 5, lSiteIndex) - pStats(1, 6, lSiteIndex)

            'volume error in lowest 50% flows
            If (pStats(2, 2, lSiteIndex) > 0.0#) Then
                pErrorTerms(3, lSiteIndex) = 100.0# * ((pStats(2, 1, lSiteIndex) - pStats(2, 2, lSiteIndex)) _
                                                  / pStats(2, 2, lSiteIndex))
            Else
                pErrorTerms(3, lSiteIndex) = Double.NaN
            End If

            'volume error in highest 10% flows
            If (pStats(3, 2, lSiteIndex) > 0.0#) Then
                pErrorTerms(4, lSiteIndex) = 100.0# * ((pStats(3, 1, lSiteIndex) - pStats(3, 2, lSiteIndex)) _
                                           / pStats(3, 2, lSiteIndex))
            Else
                pErrorTerms(4, lSiteIndex) = Double.NaN
            End If

            'total storm peaks volume
            If (pStats(5, 2, lSiteIndex) > 0.0#) Then
                pErrorTerms(11, lSiteIndex) = 100.0# * ((pStats(5, 1, lSiteIndex) - pStats(5, 2, lSiteIndex)) _
                                           / pStats(5, 2, lSiteIndex))
            Else
                pErrorTerms(11, lSiteIndex) = Double.NaN
            End If

            'total storm volume
            If (pStats(4, 2, lSiteIndex) > 0.0#) Then
                pErrorTerms(5, lSiteIndex) = 100.0# * ((pStats(4, 1, lSiteIndex) - pStats(4, 2, lSiteIndex)) _
                                           / pStats(4, 2, lSiteIndex))
            Else
                pErrorTerms(5, lSiteIndex) = Double.NaN
            End If

            'summer storm volume
            If (pStats(9, 2, lSiteIndex) > 0.0# And pStats(4, 2, lSiteIndex) > 0.0#) Then
                pErrorTerms(8, lSiteIndex) = (100.0# * ((pStats(9, 1, lSiteIndex) - pStats(9, 2, lSiteIndex)) _
                                            / pStats(9, 2, lSiteIndex))) - pErrorTerms(5, lSiteIndex)
            Else
                pErrorTerms(8, lSiteIndex) = Double.NaN
            End If

            'error in low flow recession
            If Double.IsNaN(pStats(6, 1, lSiteIndex)) Or _
               Double.IsNaN(pStats(6, 2, lSiteIndex)) Then
                pErrorTerms(2, lSiteIndex) = Double.NaN
            Else 'okay to calculate this term
                pErrorTerms(2, lSiteIndex) = (1.0# - pStats(6, 1, lSiteIndex)) - (1.0# - pStats(6, 2, lSiteIndex))
            End If

            'summer flow volume
            Dim lSummerError As Double
            If (pStats(7, 2, lSiteIndex) > 0.0#) Then
                lSummerError = 100.0# * ((pStats(7, 1, lSiteIndex) - pStats(7, 2, lSiteIndex)) _
                                           / pStats(7, 2, lSiteIndex))
            Else
                lSummerError = Double.NaN
            End If

            'winter flow volume
            Dim lWinterError As Double
            If (pStats(8, 2, lSiteIndex) > 0.0#) Then
                lWinterError = 100.0# * ((pStats(8, 1, lSiteIndex) - pStats(8, 2, lSiteIndex)) _
                                           / pStats(8, 2, lSiteIndex))
            Else
                lWinterError = Double.NaN
            End If

            'error in seasonal volume
            If (Double.IsNaN(lSummerError) Or _
                Double.IsNaN(lWinterError)) Then 'one term or the other has not been obtained
                pErrorTerms(7, lSiteIndex) = Double.NaN
            Else 'okay to calculate this term
                pErrorTerms(7, lSiteIndex) = Math.Abs(lSummerError - lWinterError)
            End If
        Next lSiteIndex

        Dim lStr As String = StatReportAsString(auci)
        Return lStr
    End Function

    Private Function StatReportAsString(ByVal aUci As atcUCI.HspfUci) As String
        Dim lStr As String
        lStr = aUci.GlobalBlock.RunInf.Value & vbCrLf
        lStr &= "Expert System Statistics for " & aUci.Name & vbCrLf
        lStr &= "Run Created: ".PadLeft(15) & FileDateTime(aUci.Name) & vbCrLf
        lStr &= "  " & aUci.GlobalBlock.RunPeriod & vbCrLf & vbCrLf

        For lSite As Integer = 1 To pExpertSystem.NSites
            'loop for each site
            lStr &= "Site: ".PadLeft(15) & pExpertSystem.SiteName(lSite) & vbCrLf & vbCrLf

            'statistics summary
            Dim lYrCnt As Double = timdifJ(pSJdate, pEJdate, 6, 1)
            lStr &= StatDetails("Total (" & lYrCnt & " year run)", lSite, 1)
            lStr &= StatDetails("Annual Average", lSite, lYrCnt)

            'Write the error terms
            lStr &= Space(35) & "Error Terms" & vbCrLf & vbCrLf
            lStr &= Space(35) & "Current".PadLeft(12) & "Criteria".PadLeft(12) & vbCrLf
            For lErrorTerm As Integer = 1 To pNErrorTerms
                If pErrorTerms(lErrorTerm, lSite) <> 0.0# Then
                    lStr &= (pErrorTermNames(lErrorTerm) & " =").PadLeft(35) & _
                            DecimalAlign(pErrorTerms(lErrorTerm, lSite)) & _
                            DecimalAlign(pExpertSystem.ErrorCriteria(lErrorTerm))
                    If Math.Abs(pErrorTerms(lErrorTerm, lSite)) < pExpertSystem.ErrorCriteria(lErrorTerm) Then
                        lStr &= " OK" & vbCrLf
                    Else
                        lStr &= "    Needs Work" & vbCrLf
                    End If
                End If
            Next lErrorTerm
            lStr &= vbCrLf & vbCrLf
        Next lSite

        Return lStr
    End Function

    Private Function StatDetails(ByVal Title As String, ByVal lSite As Integer, ByVal Conv As Double) As String
        Dim j As Integer
        Dim k As Integer
        Dim lConv As Double
        Dim lStr As String

        lStr = Space(30) & Title & vbCrLf & vbCrLf

        lStr &= Space(30) & _
              "Observed".PadLeft(15) & _
              "Simulated".PadLeft(15) & _
              "Simulated".PadLeft(15) & _
              "Simulated".PadLeft(15) & vbCrLf
        lStr &= Space(30) & _
              "Total Runoff".PadLeft(15) & _
              "Total Runoff".PadLeft(15) & _
              "Surface Runoff".PadLeft(15) & _
              "Interflow".PadLeft(15) & vbCrLf
        'Write runoff block
        For j = 1 To pStats.GetUpperBound(0)
            'loop for each error term
            lStr &= (pStatNames(j) & " =").PadLeft(30)
            Dim l() As Integer = {0, 2, 1, 3, 4} 'gets print order correct
            For k = 1 To 4
                If Not Double.IsNaN(pStats(j, l(k), lSite)) Then
                    If j = 5 Or j = 6 Then 'dont need adjustment for storm peaks or recession rate
                        lConv = 1
                    Else
                        lConv = Conv
                    End If
                    lStr &= DecimalAlign(pStats(j, l(k), lSite) / lConv, 15)
                Else
                    lStr &= Space(15)
                End If
            Next k
            lStr &= vbCrLf
        Next j
        lStr &= vbCrLf
        'Write EvapoTranspiration block
        lStr &= Space(30) & "          EvapoTranspiration" & vbCrLf
        lStr &= Space(30) & "Potential".PadLeft(15) & "Actual".PadLeft(15) & vbCrLf
        lStr &= ("total (inches) = ").PadLeft(30)
        For k = 5 To 6
            lStr &= DecimalAlign(pStats(1, k, lSite) / Conv, 15)
        Next k
        lStr &= vbCrLf & vbCrLf
        Return lStr
    End Function

    Private Sub BoxSort(ByVal maxVal As Double, ByVal minVal As Double, ByVal pNSteps As Integer, ByVal NVALS As Integer, _
                        ByVal rDat() As Double, ByVal arlgFg As Integer, _
                        ByRef aBoxCnt() As Integer, ByRef aBoxSum() As Double)
        '     Compute statistics needed by the expert system using a box sort algorithm

        '     MaxVal - maximum value in array to be sorted
        '     MinVal - minimum value in array to be sorted
        '     pNSteps - number of steps or boxes
        '     NVals  - number of values to sort
        '     RDat   - array of values to sort
        '     ArlgFg - arith/log flag, 1-arith, 2-log
        '     aBoxCnt - number of values in box
        '     aBoxSum - sum of values in box

        Dim i As Integer, lBox As Integer
        Dim incrmt As Double, rtmp As Double, rlgMin As Double, xtmp As Double

        'set box counts and box sums to zero
        aBoxCnt.Initialize() '.SetValue(0, 1, pNSteps)
        aBoxSum.Initialize() '.SetValue(0.0, 1, pNSteps)

        'set box boundaries
        If (arlgFg = 2) Then 'log of data
            rlgMin = Math.Log(minVal)
            rtmp = Math.Log(maxVal) - rlgMin
        Else 'plain data
            rlgMin = minVal
            rtmp = maxVal - minVal
        End If
        incrmt = rtmp / pNSteps
        For i = 1 To NVALS
            'find which box
            xtmp = rDat(i) + 0.000001
            'make sure value in range
            If (xtmp < minVal) Then
                'out low
                lBox = pNSteps
            ElseIf (xtmp >= maxVal) Then 'out high, put in highest ranking box
                lBox = 1
            Else 'it falls in one of the boxes in between
                If (arlgFg = 1) Then
                    rtmp = xtmp
                Else
                    rtmp = Math.Log(xtmp) - rlgMin
                End If
                lBox = pNSteps - Fix(rtmp / incrmt)
                If (lBox <= 1) Then 'max value case
                    lBox = 1
                End If
            End If
            'increment count and sum
            aBoxCnt(lBox) += 1
            aBoxSum(lBox) += rDat(i)
        Next i
    End Sub

    Private Function BoxAdd(ByVal aLimit As Integer, ByVal aBoxCnt() As Integer, _
                            ByVal aboxSum() As Double) As Double
        'Use box sort data to determine the sum of the values greater than any user specified limit.

        '     aLimit  - limit of range of values to sum (number of values to sum)
        '     aBoxCnt - number of values in box
        '     aBoxSum - sum of values in box
        '     returns - sum of values greater than limit

        Dim lIndex As Integer, lTotCnt As Integer
        Dim lSum As Double

        lSum = 0.0#
        lTotCnt = 0
        lIndex = 0
        Do 'loop through boxes starting with highest
            lIndex += 1
            lSum += aboxSum(lIndex)
            lTotCnt += aBoxCnt(lIndex)
        Loop While (lTotCnt < aLimit)
        'Compensate for inexact number of values by interpolating, subtract
        'off a proportional amount of the box containing the cut-off value.
        lSum -= (lTotCnt - aLimit) * aboxSum(lIndex) / aBoxCnt(lIndex)
        Return lSum
    End Function

    Private Sub ZipR(ByVal aLength As Long, ByVal aZip As Single, ByVal lArray(,,) As Double, _
                     ByVal aSecondDim As Long, ByVal aThirdDim As Long)
        'Fill the real array X of size Length with the given value ZIP.
        For lIndex As Integer = 1 To aLength
            lArray(lIndex, aSecondDim, aThirdDim) = aZip
        Next lIndex
    End Sub
End Module
