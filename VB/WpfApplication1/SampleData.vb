Imports System
Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm

Namespace WpfApplication1

    Public Class SampleDataGenerator

        Private _Appointments As ObservableCollection(Of WpfApplication1.AppointmentData), _Resources As ObservableCollection(Of WpfApplication1.ResourceData)

        Private Shared Statuses As Integer() = New Integer() {0, 2, 3, 4}

        Private Shared Labels As Integer() = New Integer() {1, 2, 4, 6, 8}

        Private Shared rnd As Random = New Random()

        Private Shared Function CreateResource(ByVal i As Integer) As ResourceData
            Dim resource As ResourceData = New ResourceData()
            resource.Caption = String.Format("Resource {0}", i + 1)
            resource.Id = i
            Return resource
        End Function

        Private Shared Function CreateAppointment(ByVal i As Integer, ByVal startDate As Date, ByVal duration As TimeSpan, ByVal resourceCount As Integer) As AppointmentData
            Dim start As Date = startDate.Add(New TimeSpan(rnd.Next(0, 23), rnd.Next(0, 60), 0))
            Dim res As AppointmentData = New AppointmentData()
            res.Id = i
            res.StartTime = start
            res.EndTime = start + duration
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

        Private startDate As Date

        Private endDate As Date

        Public Sub New()
            Me.New(Date.Today, False)
        End Sub

        Public Sub New(ByVal startDate As Date, ByVal useAllDayAppointments As Boolean)
            Appointments = New ObservableCollection(Of AppointmentData)()
            Resources = New ObservableCollection(Of ResourceData)()
            Me.startDate = startDate
            endDate = startDate
            appointmentsPerDay = 0
            Me.useAllDayAppointments = useAllDayAppointments
        End Sub

        Public Property Appointments As ObservableCollection(Of AppointmentData)
            Get
                Return _Appointments
            End Get

            Private Set(ByVal value As ObservableCollection(Of AppointmentData))
                _Appointments = value
            End Set
        End Property

        Public Property Resources As ObservableCollection(Of ResourceData)
            Get
                Return _Resources
            End Get

            Private Set(ByVal value As ObservableCollection(Of ResourceData))
                _Resources = value
            End Set
        End Property

        Public Sub Clear()
            Appointments.Clear()
            Resources.Clear()
            endDate = startDate
            appointmentsPerDay = 0
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
                endDate = startDate
                UpdateDayCount(dayCount)
                Return
            End If

            UpdateDayCount(dayCount)
            If resourcesUpdated Then UpdateAppointmentResources()
        End Sub

        Private Function UpdateResources(ByVal newResourceCount As Integer) As Boolean
            If newResourceCount = Resources.Count Then Return False
            Dim oldResourceCount As Integer = Resources.Count()
            For i As Integer = 0 To oldResourceCount - newResourceCount - 1
                Resources.RemoveAt(Resources.Count - 1)
            Next

            For i As Integer = 0 To newResourceCount - oldResourceCount - 1
                Resources.Add(CreateResource(Resources.Count))
            Next

            Return True
        End Function

        Private Function UpdateDayCount(ByVal newDayCount As Integer) As Boolean
            Dim newEndDate As Date = startDate.AddDays(newDayCount)
            If newEndDate.Equals(endDate) Then Return False
            Dim [date] As Date = endDate
            While [date] > newEndDate
                For i As Integer = 0 To appointmentsPerDay - 1
                    Appointments.RemoveAt(Appointments.Count - 1)
                Next

                [date] -= TimeSpan.FromDays(1)
            End While

            Dim [date] As Date = endDate
            While [date] < newEndDate
                For i As Integer = 0 To appointmentsPerDay - 1
                    Appointments.Add(CreateAppointment(Appointments.Count - 1, [date], CalculateDuration(i), Resources.Count))
                Next

                [date] += TimeSpan.FromDays(1)
            End While

            endDate = newEndDate
            Return True
        End Function

        Private Function CalculateDuration(ByVal i As Integer) As TimeSpan
            Return If(useAllDayAppointments AndAlso i Mod 10 = 0, TimeSpan.FromDays(rnd.Next(0, 5)), TimeSpan.FromMinutes(rnd.Next(0, 60)))
        End Function

        Private Sub UpdateAppointmentResources()
            For Each apt As AppointmentData In Appointments
                apt.ResourceId = rnd.Next(0, Resources.Count)
            Next
        End Sub
    End Class

    Public Class AppointmentData
        Inherits BindableBase

        Private Shared ResourceIdName As String = GetPropertyName(Function() CType(Nothing, AppointmentData).ResourceId)

        Private resourceIdField As Integer

        Public Sub New()
        End Sub

        Public Property StartTime As Date

        Public Property EndTime As Date

        Public Property Subject As String

        Public Property Location As String

        Public Property Description As String

        Public Property TimeZoneId As String

        Public Property Id As Integer

        Public Property LabelKey As Object

        Public Property StatusKey As Object

        Public Property AllDay As Boolean

        Public Property ReminderInfo As String

        Public Property Type As Integer?

        Public Property RecurrenceInfo As String

        Public Property ResourceId As Integer
            Get
                Return resourceIdField
            End Get

            Set(ByVal value As Integer)
                SetProperty(resourceIdField, value, ResourceIdName)
            End Set
        End Property
    End Class

    Public Class ResourceData

        Public Sub New()
        End Sub

        Public Property Caption As String

        Public Property Id As Integer
    End Class
End Namespace
