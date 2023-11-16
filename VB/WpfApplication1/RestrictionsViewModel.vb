Imports System.Collections.ObjectModel
Imports DevExpress.Mvvm.POCO

Namespace WpfApplication1

    Public Class RestrictionsViewModel

        Private ReadOnly data As SampleDataGenerator = New SampleDataGenerator()

        Protected Sub New()
            AppointmentsPerDay = 10
            DayCount = 2
            ResourceCount = 3
        End Sub

        Public Overridable ReadOnly Property Appointments As ObservableCollection(Of AppointmentData)
            Get
                Return data.Appointments
            End Get
        End Property

        Public Overridable ReadOnly Property Resources As ObservableCollection(Of ResourceData)
            Get
                Return data.Resources
            End Get
        End Property

        Public Overridable Property AppointmentsPerDay As Integer

        Public Overridable Property DayCount As Integer

        Public Overridable Property ResourceCount As Integer

        Protected Sub OnDayCountChanged()
            data.SetUp(DayCount, ResourceCount, AppointmentsPerDay)
        End Sub

        Protected Sub OnResourceCountChanged()
            data.SetUp(DayCount, ResourceCount, AppointmentsPerDay)
        End Sub

        Public Shared Function Create() As RestrictionsViewModel
            Return ViewModelSource.Create(Function() New RestrictionsViewModel())
        End Function
    End Class
End Namespace
