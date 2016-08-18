using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC5IdentitySample.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [DisplayName("ユーザ名")]
        [Required]
        public string UserName { get; set; }

        [DisplayName("メモ")]
        public string Memo { get; set; }
    }
}