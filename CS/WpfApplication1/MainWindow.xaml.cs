using System;
using System.Windows;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduling;
using DevExpress.Mvvm;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        TimeInterval lunchTime = new TimeInterval(DateTime.Today.AddHours(12), new TimeSpan(1, 00, 00));
        public MainWindow()
        {
            InitializeComponent();
        }        

        #region #CustomAllowAppointmentCreate
        private void schedulerControl1_CustomAllowAppointmentCreate(object sender, AppointmentItemOperationEventArgs e)
        {
            //Retrieve the selected interval:
            DateTimeRange selectedIntervalRange = schedulerControl1.SelectedInterval;
            TimeInterval selectedInterval = new TimeInterval(selectedIntervalRange.Start, selectedIntervalRange.End);

            //Check whether the selected interval intersects with the resticted interval:
            //If true, restrict appointment creation 
            e.Allow = IsIntervalAllowed(selectedInterval);
        }
        #endregion #CustomAllowAppointmentCreate

        #region #CustomAllowAppointmentConflicts
        private void schedulerControl1_CustomAllowAppointmentConflicts(object sender, AppointmentItemConflictEventArgs e)
        {
            //Obtain the selected interval:
            TimeInterval interval = e.Interval;

            //If the appointment is to be moved to the restricted time interval, 
            //Add the dragged appointment the conflicting appointments collection: 
            if (!IsIntervalAllowed(interval))
                e.Conflicts.Add(e.AppointmentClone);
        }
            #endregion #CustomAllowAppointmentConflicts 
			
		#region #IsIntervalAllowed
        //This method is used to check 
		//whether the target interval intersects with the resticted interval:
		private bool IsIntervalAllowed(TimeInterval interval)
        {
            DateTime dayStart = interval.Start.Date;
            
            while (dayStart < interval.End)
            {
                if (interval.IntersectsWithExcludingBounds(lunchTime))
                    return false;
                dayStart = dayStart.AddDays(1);
            }
            return true;
        }
        #endregion #IsIntervalAllowed
        
        #region #Events
        private void BarButtonItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (barCheckItem1.IsChecked == true)
            {schedulerControl1.CustomAllowAppointmentCreate -= schedulerControl1_CustomAllowAppointmentCreate;}
            else {schedulerControl1.CustomAllowAppointmentCreate += schedulerControl1_CustomAllowAppointmentCreate;}
        }

        private void barCheckItem2_CheckedChanged(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (barCheckItem2.IsChecked == true)
            {
                schedulerControl1.CustomAllowAppointmentConflicts -= schedulerControl1_CustomAllowAppointmentConflicts;
            }
            else
            {
                schedulerControl1.CustomAllowAppointmentConflicts += schedulerControl1_CustomAllowAppointmentConflicts;
            }

        }
        #endregion #Events	
    
	}	
}