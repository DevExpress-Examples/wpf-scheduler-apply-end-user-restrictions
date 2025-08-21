Imports System
Imports System.Windows
Imports DevExpress.XtraScheduler
Imports DevExpress.Xpf.Scheduling
Imports DevExpress.Mvvm

Namespace WpfApplication1

    Public Partial Class MainWindow
        Inherits Window

        Private lunchTime As TimeInterval = New TimeInterval(Date.Today.AddHours(12), New TimeSpan(1, 00, 00))

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#Region "#CustomAllowAppointmentCreate"
        Private Sub customAllowAppointmentCreateHandler(ByVal sender As Object, ByVal e As AppointmentItemOperationEventArgs)
            If CBool(Me.barItemDisableCreatingAppointments.IsChecked) Then
                ' If the "Disable Creating Appointments" bar item is checked, do not allow appointment creation
                e.Allow = False
                Return
            End If

            Dim selectedIntervalRange As DateTimeRange = Me.schedulerControl1.SelectedInterval
            Dim selectedInterval As TimeInterval = New TimeInterval(selectedIntervalRange.Start, selectedIntervalRange.End)
            e.Allow = IsIntervalAllowed(selectedInterval)
        End Sub

#End Region  ' #CustomAllowAppointmentCreate
#Region "#CustomAllowAppointmentConflicts"
        Private Sub customAllowAppointmentConflictsHandler(ByVal sender As Object, ByVal e As AppointmentItemConflictEventArgs)
            If CBool(Me.barItemDisableAppointmentConflicts.IsChecked) Then
                e.Conflicts.Clear()
                Return
            End If

            Dim interval As TimeInterval = e.Interval
            If Not IsIntervalAllowed(interval) Then e.Conflicts.Add(e.AppointmentClone)
        End Sub

#End Region  ' #CustomAllowAppointmentConflicts 
#Region "#IsIntervalAllowed"
        ' This method checks whether the target interval intersects with the restricted interval
        Private Function IsIntervalAllowed(ByVal interval As TimeInterval) As Boolean
            Dim dayStart As Date = interval.Start.Date
            While dayStart < interval.End
                If interval.IntersectsWithExcludingBounds(lunchTime) Then Return False
                dayStart = dayStart.AddDays(1)
            End While

            Return True
        End Function
#End Region  ' #IsIntervalAllowed
    End Class
End Namespace
