using System;

namespace SplititScraperApi.Models
{
    public class Actor
    {
        public int Id { get; set;}
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Actor name is emptuy");
                }
                _name = value;
            }
        }

        public int Rank { get; set; }
        public string Bio { get; set; }
    }
}
