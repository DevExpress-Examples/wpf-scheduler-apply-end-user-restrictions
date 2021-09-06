Imports System
Imports System.Collections.ObjectModel
Imports System.Linq
Imports DevExpress.Mvvm

Namespace WpfApplication1
	Public Class SampleDataGenerator
		Private Shared Statuses() As Integer = { 0, 2, 3, 4 }
		Private Shared Labels() As Integer = { 1, 2, 4, 6, 8 }
		Private Shared rnd As New Random()

		Private Shared Function CreateResource(ByVal i As Integer) As ResourceData
			Dim resource As New ResourceData()
			resource.Caption = String.Format("Resource {0}", i + 1)
			resource.Id = i
			Return resource
		End Function

		Private Shared Function CreateAppointment(ByVal i As Integer, ByVal startDate As DateTime, ByVal duration As TimeSpan, ByVal resourceCount As Integer) As AppointmentData
			Dim start As DateTime = startDate.Add(New TimeSpan(rnd.Next(0, 23), rnd.Next(0, 60), 0))
			Dim res As New AppointmentData()
			res.Id = i
			res.StartTime = start
			res.EndTime = start.Add(duration)
			res.Subject = String.Format("Apt #{0}", i + 1)
			res.Location = String.Format("Location {0}", i + 1)
			res.Description = String.Format("Appointment Description {0}", i + 1)
			res.LabelKey = Labels(rnd.Next(0, 4))
			res.ResourceId = rnd.Next(0, resourceCount)
			res.StatusKey = Statuses(rnd.Next(0, 3))
			Return res
		End Function

		Private appointmentsPerDay As Integer
		Private useAllDayAppointments As Boolean

		Private startDate As DateTime
		Private endDate As DateTime
		Public Sub New()
			Me.New(DateTime.Today, False)
		End Sub
		Public Sub New(ByVal startDate As DateTime, ByVal useAllDayAppointments As Boolean)
			Appointments = New ObservableCollection(Of AppointmentData)()
			Resources = New ObservableCollection(Of ResourceData)()
			Me.startDate = startDate
			Me.endDate = startDate
			Me.appointmentsPerDay = 0
			Me.useAllDayAppointments = useAllDayAppointments
		End Sub

		Private privateAppointments As ObservableCollection(Of AppointmentData)
		Public Property Appointments() As ObservableCollection(Of AppointmentData)
			Get
				Return privateAppointments
			End Get
			Private Set(ByVal value As ObservableCollection(Of AppointmentData))
				privateAppointments = value
			End Set
		End Property
		Private privateResources As ObservableCollection(Of ResourceData)
		Public Property Resources() As ObservableCollection(Of ResourceData)
			Get
				Return privateResources
			End Get
			Private Set(ByVal value As ObservableCollection(Of ResourceData))
				privateResources = value
			End Set
		End Property

		Public Sub Clear()
			Appointments.Clear()
			Resources.Clear()
			Me.endDate = Me.startDate
			Me.appointmentsPerDay = 0
		End Sub

		Public Sub SetUp(ByVal dayCount As Integer, ByVal resourceCount As Integer, ByVal appointmentsPerDay As Integer)
			If dayCount <= 0 OrElse resourceCount <= 0 OrElse appointmentsPerDay <= 0 Then
				Clear()
				Return
			End If
			Dim appointmentsPerDayChanged As Boolean = Me.appointmentsPerDay <> appointmentsPerDay
			Dim resourcesUpdated As Boolean = UpdateResources(resourceCount)
			If appointmentsPerDayChanged Then
				Me.appointmentsPerDay = appointmentsPerDay
				Appointments.Clear()
				Me.endDate = Me.startDate
				UpdateDayCount(dayCount)
				Return
			End If
			UpdateDayCount(dayCount)
			If resourcesUpdated Then
				UpdateAppointmentResources()
			End If
		End Sub

		Private Function UpdateResources(ByVal newResourceCount As Integer) As Boolean
			If newResourceCount = Resources.Count Then
				Return False
			End If
			Dim oldResourceCount As Integer = Resources.Count()
			Dim i As Integer = 0
			Do While i < oldResourceCount - newResourceCount
				Resources.RemoveAt(Resources.Count - 1)
				i += 1
			Loop
			i = 0
			Do While i < newResourceCount - oldResourceCount
				Resources.Add(CreateResource(Resources.Count))
				i += 1
			Loop
			Return True
		End Function

		Private Function UpdateDayCount(ByVal newDayCount As Integer) As Boolean
			Dim newEndDate As DateTime = Me.startDate.AddDays(newDayCount)
			If newEndDate.Equals(Me.endDate) Then
				Return False
			End If
			Dim [date] As DateTime = Me.endDate
			Do While [date] > newEndDate
				For i As Integer = 0 To Me.appointmentsPerDay - 1
					Appointments.RemoveAt(Appointments.Count - 1)
				Next i
				[date] = [date].Subtract(TimeSpan.FromDays(1))
			Loop
			[date] = Me.endDate
			Do While [date] < newEndDate
				For i As Integer = 0 To Me.appointmentsPerDay - 1
					Appointments.Add(CreateAppointment(Appointments.Count - 1, [date], CalculateDuration(i), Resources.Count))
				Next i
				[date] = [date].Add(TimeSpan.FromDays(1))
			Loop
			Me.endDate = newEndDate
			Return True
		End Function

		Private Function CalculateDuration(ByVal i As Integer) As TimeSpan
			Return If(Me.useAllDayAppointments AndAlso i Mod 10 = 0, TimeSpan.FromDays(rnd.Next(0, 5)), TimeSpan.FromMinutes(rnd.Next(0, 60)))
		End Function

		Private Sub UpdateAppointmentResources()
			For Each apt As AppointmentData In Appointments
				apt.ResourceId = rnd.Next(0, Resources.Count)
			Next apt
		End Sub
	End Class

	Public Class AppointmentData
		Inherits BindableBase

		Private Shared ResourceIdName As String = GetPropertyName(Function() (DirectCast(Nothing, AppointmentData)).ResourceId)

'INSTANT VB NOTE: The field resourceId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private resourceId_Conflict As Integer
		Public Sub New()
		End Sub

		Public Property StartTime() As DateTime
		Public Property EndTime() As DateTime
		Public Property Subject() As String
		Public Property Location() As String
		Public Property Description() As String
		Public Property TimeZoneId() As String
		Public Property Id() As Integer
		Public Property LabelKey() As Object
		Public Property StatusKey() As Object
		Public Property AllDay() As Boolean
		Public Property ReminderInfo() As String
		Public Property Type() As Integer?
		Public Property RecurrenceInfo() As String
		Public Property ResourceId() As Integer
			Get
				Return Me.resourceId_Conflict
			End Get
			Set(ByVal value As Integer)
				SetProperty(Me.resourceId_Conflict, value, ResourceIdName)
			End Set
		End Property
	End Class
	Public Class ResourceData
		Public Sub New()
		End Sub

		Public Property Caption() As String
		Public Property Id() As Integer
	End Class
End Namespace
