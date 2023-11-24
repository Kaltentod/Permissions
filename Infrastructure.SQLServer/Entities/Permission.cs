using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Permissions.Infrastructure.SQLServer.Entities
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string EmployeeForename { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string EmployeeSurname { get; set; }

        [Required]
        public int PermissionType { get; set; }

        [Required]
        public DateTime PermissionDate { get; set; }

        [ForeignKey("PermissionType")]
        public virtual PermissionType Type { get; set; }
    }
}
