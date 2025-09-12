<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128656107/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T565891)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# WPF Scheduler - Apply User Restrictions

This example applies the following user restrictions:

* Prevents users from creating new appointments.
* Prevents users from moving appointments to the lunch break interval (12:00–13:00).

In this example, the Ribbon UI displays **Disable Creating Appointments** and **Disable Appointment Conflict** controls that toggle these restrictions.

![Apply User Restrictions - WPF Scheduler, DevExpress](./Images/scheduler.jpg)

## Implementation Details

### Define Restricted Interval

In this example, the restricted interval covers the lunch break time (12:00–13:00):

```csharp
TimeInterval lunchTime = new TimeInterval(DateTime.Today.AddHours(12), TimeSpan.FromHours(1));
```

The `IsIntervalAllowed` method returns `false` if any part of the selected interval overlaps with the restricted time range:

```csharp
private bool IsIntervalAllowed(TimeInterval interval) {
    DateTime dayStart = interval.Start.Date;
    while (dayStart < interval.End) {
        if (interval.IntersectsWithExcludingBounds(lunchTime))
            return false;
        dayStart = dayStart.AddDays(1);
    }
    return true;
}
```

### Restrict Appointment Creation

The [`CustomAllowAppointmentCreate`](https://docs.devexpress.com/WPF/DevExpress.Xpf.Scheduling.SchedulerControl.CustomAllowAppointmentCreate) event allows you to define rules that restrict appointment creation. In this example, the event handler:

* Blocks appointment creation if **Disable Creating Appointments** is turned on.
* Prevents users from creating appointments during lunch time if **Disable Creating Appointments** is turned off.

```csharp
private void customAllowAppointmentCreateHandler(object sender, AppointmentItemOperationEventArgs e) {
    // Cancel the operation if the "Disable Creating Appointments" bar item is checked
    if((bool)barItemDisableCreatingAppointments.IsChecked) {
        e.Allow = false;
        return;
    }
    DateTimeRange selectedIntervalRange = schedulerControl1.SelectedInterval;
    TimeInterval selectedInterval = new TimeInterval(selectedIntervalRange.Start, selectedIntervalRange.End);
    e.Allow = IsIntervalAllowed(selectedInterval);
}
```

The `barItemDisableCreatingAppointments.IsChecked` flag is bound to the **Disable Creating Appointments** Ribbon item and acts as a switch.

### Resolve Appointment Conflicts

The [CustomAllowAppointmentConflicts](https://docs.devexpress.com/WPF/DevExpress.Xpf.Scheduling.SchedulerControl.CustomAllowAppointmentConflicts) event allows you to control how the scheduler handles appointment conflicts. In this example, a conflict occurs when a user moves an appointment to a time interval that already contains another appointment or to the lunch break time. In the Ribbon UI, you can enable the **Disable Appointment Conflicts** item to ignore conflicts.

```csharp
private void customAllowAppointmentConflictsHandler(object sender, AppointmentItemConflictEventArgs e) {
    if((bool)barItemDisableAppointmentConflicts.IsChecked) { 
        e.Conflicts.Clear(); // clear all restrictions
        return;
    }

    TimeInterval interval = e.Interval;

    if (!IsIntervalAllowed(interval))
        e.Conflicts.Add(e.AppointmentClone);
}
```

The `barItemDisableAppointmentConflicts.IsChecked` flag is bound to a custom Ribbon item (**Disable Appointment Conflict**) that toggles this behavior.

## Files to Review

* [MainWindow.xaml](./CS/WpfApplication1/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/WpfApplication1/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/WpfApplication1/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/WpfApplication1/MainWindow.xaml.vb))
* [RestrictionsViewModel.cs](./CS/WpfApplication1/RestrictionsViewModel.cs) (VB: [RestrictionsViewModel.vb](./VB/WpfApplication1/RestrictionsViewModel.vb))
* [SampleData.cs](./CS/WpfApplication1/SampleData.cs) (VB: [SampleData.vb](./VB/WpfApplication1/SampleData.vb))

## Documentation

* [Scheduler](https://docs.devexpress.com/WPF/114881/controls-and-libraries/scheduler)
* [End-User Restrictions](https://docs.devexpress.com/WPF/119359/controls-and-libraries/scheduler/end-user-restrictions)
* [CustomAllowAppointmentCreate](https://docs.devexpress.com/WPF/DevExpress.Xpf.Scheduling.SchedulerControl.CustomAllowAppointmentCreate)
* [CustomAllowAppointmentConflicts](https://docs.devexpress.com/WPF/DevExpress.Xpf.Scheduling.SchedulerControl.CustomAllowAppointmentConflicts)

## More Examples

* [WPF Scheduler - Customize the Built-In Ribbon Control](https://github.com/DevExpress-Examples/wpf-scheduler-customize-built-in-ribbon-control)
* [WPF Scheduler - Specify Custom Work Time Intervals](https://github.com/DevExpress-Examples/wpf-scheduler-specify-custom-work-time-intervals)
* [WPF Scheduler - Filter Time Regions](https://github.com/DevExpress-Examples/wpf-scheduler-filter-time-regions)
* [WPF Scheduler - ICalendar Support](https://github.com/DevExpress-Examples/wpfscheduler-provide-icalendar-data-exchange-functionality)
* [WPF Scheduler - Apply User Restrictions](https://github.com/DevExpress-Examples/wpf-scheduler-apply-end-user-restrictions)

<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-scheduler-apply-end-user-restrictions&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=wpf-scheduler-apply-end-user-restrictions&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
