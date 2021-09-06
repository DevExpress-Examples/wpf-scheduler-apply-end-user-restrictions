Imports System
Imports System.Windows
Imports DevExpress.XtraScheduler
Imports DevExpress.Xpf.Scheduling
Imports DevExpress.Mvvm

Namespace WpfApplication1
	Partial Public Class MainWindow
		Inherits Window

		Private lunchTime As New TimeInterval(DateTime.Today.AddHours(12), New TimeSpan(1, 00, 00))
		Public Sub New()
			InitializeComponent()
		End Sub

		#Region "#CustomAllowAppointmentCreate"
		Private Sub schedulerControl1_CustomAllowAppointmentCreate(ByVal sender As Object, ByVal e As AppointmentItemOperationEventArgs)
			'Retrieve the selected interval:
			Dim selectedIntervalRange As DateTimeRange = schedulerControl1.SelectedInterval
			Dim selectedInterval As New TimeInterval(selectedIntervalRange.Start, selectedIntervalRange.End)

			'Check whether the selected interval intersects with the resticted interval:
			'If true, restrict appointment creation 
			e.Allow = IsIntervalAllowed(selectedInterval)
		End Sub
		#End Region ' #CustomAllowAppointmentCreate

		#Region "#CustomAllowAppointmentConflicts"
		Private Sub schedulerControl1_CustomAllowAppointmentConflicts(ByVal sender As Object, ByVal e As AppointmentItemConflictEventArgs)
			'Obtain the selected interval:
			Dim interval As TimeInterval = e.Interval

			'If the appointment is to be moved to the restricted time interval, 
			'Add the dragged appointment the conflicting appointments collection: 
			If Not IsIntervalAllowed(interval) Then
				e.Conflicts.Add(e.AppointmentClone)
			End If
		End Sub
			#End Region ' #CustomAllowAppointmentConflicts 

		#Region "#IsIntervalAllowed"
		'This method is used to check 
		'whether the target interval intersects with the resticted interval:
		Private Function IsIntervalAllowed(ByVal interval As TimeInterval) As Boolean
			Dim dayStart As DateTime = interval.Start.Date

			Do While dayStart < interval.End
				If interval.IntersectsWithExcludingBounds(lunchTime) Then
					Return False
				End If
				dayStart = dayStart.AddDays(1)
			Loop
			Return True
		End Function
		#End Region ' #IsIntervalAllowed

		#Region "#Events"
		Private Sub BarButtonItem_ItemClick(ByVal sender As Object, ByVal e As DevExpress.Xpf.Bars.ItemClickEventArgs)
			If barCheckItem1.IsChecked = True Then
				RemoveHandler schedulerControl1.CustomAllowAppointmentCreate, AddressOf schedulerControl1_CustomAllowAppointmentCreate
			Else
				AddHandler schedulerControl1.CustomAllowAppointmentCreate, AddressOf schedulerControl1_CustomAllowAppointmentCreate
			End If
		End Sub

		Private Sub barCheckItem2_CheckedChanged(ByVal sender As Object, ByVal e As DevExpress.Xpf.Bars.ItemClickEventArgs)
			If barCheckItem2.IsChecked = True Then
				RemoveHandler schedulerControl1.CustomAllowAppointmentConflicts, AddressOf schedulerControl1_CustomAllowAppointmentConflicts
			Else
				AddHandler schedulerControl1.CustomAllowAppointmentConflicts, AddressOf schedulerControl1_CustomAllowAppointmentConflicts
			End If

		End Sub
		#End Region ' #Events    

	End Class
End Namespace