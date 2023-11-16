using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraScheduler;

namespace WpfApplication1 {
    public class RestrictionsViewModel {
        readonly SampleDataGenerator data = new SampleDataGenerator();
        protected RestrictionsViewModel() {
            AppointmentsPerDay = 10;
            DayCount = 2;
            ResourceCount = 3;
        }

        public virtual ObservableCollection<AppointmentData> Appointments { get { return data.Appointments; } }
        public virtual ObservableCollection<ResourceData> Resources { get { return data.Resources; } }
        public virtual int AppointmentsPerDay { get; set; }
        public virtual int DayCount { get; set; }
        public virtual int ResourceCount { get; set; }
        protected void OnDayCountChanged() {
            data.SetUp(DayCount, ResourceCount, AppointmentsPerDay);
        }
        protected void OnResourceCountChanged() {
            data.SetUp(DayCount, ResourceCount, AppointmentsPerDay);
        }
        public static RestrictionsViewModel Create() {
            return ViewModelSource.Create(() => new RestrictionsViewModel());
        }
    }
}
