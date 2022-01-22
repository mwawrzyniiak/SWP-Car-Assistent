namespace SWPCarAssistent.Core.Common.Entities
{
    public class StartupParams
    {
        public StartupParams() { }

        public int Id { get; set; }

        public bool Lights { get; set; }

        public bool Wipers { get; set; }

        public bool CarWindows { get; set; }

        public bool Radio { get; set; }

        public bool AirConditioning { get; set; }

        public bool Heating { get; set; }

        public StartupParams(int id, bool lights, bool wipers, bool carWindows, bool radio, bool airConditioning, bool heating)
        {
            Id = id;
            Lights = lights;
            Wipers = wipers;
            CarWindows = carWindows;
            Radio = radio;
            AirConditioning = airConditioning;
            Heating = heating;
        }
    }
}
