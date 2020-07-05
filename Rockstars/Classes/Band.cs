using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rockstars.Classes
{
    public class Band
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
