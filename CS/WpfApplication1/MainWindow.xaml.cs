using System;
using System.Windows;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduling;
using DevExpress.Mvvm;

namespace WpfApplication1 {
    public partial class MainWindow : Window {
        TimeInterval lunchTime = new TimeInterval(DateTime.Today.AddHours(12), new TimeSpan(1, 00, 00));
        public MainWindow() {
            InitializeComponent();
        }

        #region #CustomAllowAppointmentCreate
        private void customAllowAppointmentCreateHandler(object sender, AppointmentItemOperationEventArgs e) {
            if((bool)barItemDisableCreatingAppointments.IsChecked) {
                //If the "Disable Creating Appointments" bar item is checked, do not allow appointment creation.
                e.Allow = false;
                return;
            }
            DateTimeRange selectedIntervalRange = schedulerControl1.SelectedInterval;
            TimeInterval selectedInterval = new TimeInterval(selectedIntervalRange.Start, selectedIntervalRange.End);
            e.Allow = IsIntervalAllowed(selectedInterval);
        }
        #endregion #CustomAllowAppointmentCreate

        #region #CustomAllowAppointmentConflicts
        private void customAllowAppointmentConflictsHandler(object sender, AppointmentItemConflictEventArgs e) {
            if((bool)barItemDisableAppointmentConflicts.IsChecked) { 
                e.Conflicts.Clear();
                return;
            }

            TimeInterval interval = e.Interval;

            if (!IsIntervalAllowed(interval))
                e.Conflicts.Add(e.AppointmentClone);
        }
        #endregion #CustomAllowAppointmentConflicts 

        #region #IsIntervalAllowed
        //This method checks whether the target interval intersects with the restricted interval
        private bool IsIntervalAllowed(TimeInterval interval) {
            DateTime dayStart = interval.Start.Date;

            while (dayStart < interval.End) {
                if (interval.IntersectsWithExcludingBounds(lunchTime))
                    return false;
                dayStart = dayStart.AddDays(1);
            }
            return true;
        }
        #endregion #IsIntervalAllowed
    }
}
