using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;

namespace WpfApplication1 {
    public class SampleDataGenerator {
        static int[] Statuses = new int[] { 0, 2, 3, 4 };
        static int[] Labels = new int[] { 1, 2, 4, 6, 8 };
        static Random rnd = new Random();

        static ResourceData CreateResource(int i) {
            ResourceData resource = new ResourceData();
            resource.Caption = String.Format("Resource {0}", i + 1);
            resource.Id = i;
            return resource;
        }

        static AppointmentData CreateAppointment(int i, DateTime startDate, TimeSpan duration, int resourceCount) {
            DateTime start = startDate.Add(new TimeSpan(rnd.Next(0, 23), rnd.Next(0, 60), 0));
            AppointmentData res = new AppointmentData();
            res.Id = i;
            res.StartTime = start;
            res.EndTime = start + duration;
            res.Subject = String.Format("Apt #{0}", i + 1);
            res.Location = String.Format("Location {0}", i + 1);
            res.Description = String.Format("Appointment Description {0}", i + 1);
            res.LabelKey = Labels[rnd.Next(0, 4)];
            res.ResourceId = rnd.Next(0, resourceCount);
            res.StatusKey = Statuses[rnd.Next(0, 3)];
            return res;
        }

        int appointmentsPerDay;
        bool useAllDayAppointments;

        DateTime startDate;
        DateTime endDate;
        public SampleDataGenerator() : this(DateTime.Today, false) { }
        public SampleDataGenerator(DateTime startDate, bool useAllDayAppointments) {
            Appointments = new ObservableCollection<AppointmentData>();
            Resources = new ObservableCollection<ResourceData>();
            this.startDate = startDate;
            this.endDate = startDate;
            this.appointmentsPerDay = 0;
            this.useAllDayAppointments = useAllDayAppointments;
        }

        public ObservableCollection<AppointmentData> Appointments { get; private set; }
        public ObservableCollection<ResourceData> Resources { get; private set; }

        public void Clear() {
            Appointments.Clear();
            Resources.Clear();
            this.endDate = this.startDate;
            this.appointmentsPerDay = 0;
        }

        public void SetUp(int dayCount, int resourceCount, int appointmentsPerDay) {
            if (dayCount <= 0 || resourceCount <= 0 || appointmentsPerDay <= 0) {
                Clear();
                return;
            }
            bool appointmentsPerDayChanged = this.appointmentsPerDay != appointmentsPerDay;
            bool resourcesUpdated = UpdateResources(resourceCount);
            if (appointmentsPerDayChanged) {
                this.appointmentsPerDay = appointmentsPerDay;
                Appointments.Clear();
                this.endDate = this.startDate;
                UpdateDayCount(dayCount);
                return;
            }
            UpdateDayCount(dayCount);
            if (resourcesUpdated)
                UpdateAppointmentResources();
        }

        bool UpdateResources(int newResourceCount) {
            if (newResourceCount == Resources.Count)
                return false;
            int oldResourceCount = Resources.Count();
            for (int i = 0; i < oldResourceCount - newResourceCount; i++)
                Resources.RemoveAt(Resources.Count - 1);
            for (int i = 0; i < newResourceCount - oldResourceCount; i++)
                Resources.Add(CreateResource(Resources.Count));
            return true;
        }

        bool UpdateDayCount(int newDayCount) {
            DateTime newEndDate = this.startDate.AddDays(newDayCount);
            if (newEndDate.Equals(this.endDate))
                return false;
            for (DateTime dateRemove = this.endDate; dateRemove > newEndDate; dateRemove -= TimeSpan.FromDays(1))
                for (int i = 0; i < this.appointmentsPerDay; i++)
                    Appointments.RemoveAt(Appointments.Count - 1);
            for (DateTime dateAdd = this.endDate; dateAdd < newEndDate; dateAdd += TimeSpan.FromDays(1))
                for (int i = 0; i < this.appointmentsPerDay; i++)
                    Appointments.Add(CreateAppointment(Appointments.Count - 1, dateAdd, CalculateDuration(i), Resources.Count));
            this.endDate = newEndDate;
            return true;
        }

        TimeSpan CalculateDuration(int i) {
            return (this.useAllDayAppointments && i % 10 == 0) ? TimeSpan.FromDays(rnd.Next(0, 5)) : TimeSpan.FromMinutes(rnd.Next(0, 60));
        }

        void UpdateAppointmentResources() {
            foreach (AppointmentData apt in Appointments)
                apt.ResourceId = rnd.Next(0, Resources.Count);
        }
    }

    public class AppointmentData : BindableBase {
        static string ResourceIdName = GetPropertyName(() => ((AppointmentData)null).ResourceId);

        int resourceId;
        public AppointmentData() { }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string TimeZoneId { get; set; }
        public int Id { get; set; }
        public object LabelKey { get; set; }
        public object StatusKey { get; set; }
        public bool AllDay { get; set; }
        public string ReminderInfo { get; set; }
        public int? Type { get; set; }
        public string RecurrenceInfo { get; set; }
        public int ResourceId { get { return this.resourceId; } set { SetProperty(ref this.resourceId, value, ResourceIdName); } }
    }
    public class ResourceData {
        public ResourceData() { }

        public string Caption { get; set; }
        public int Id { get; set; }
    }
}
