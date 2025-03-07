namespace Services.ApiModels.MasterSchedule
{
    public class MasterSchedulesListDTO
    {
        public DateOnly Date { get; set; }
        public List<MasterScheduleDTO> Schedules { get; set; }

        public MasterSchedulesListDTO()
        {
            Schedules = new List<MasterScheduleDTO>();
        }
    }
}