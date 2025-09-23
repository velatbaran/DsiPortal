using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class MyProfileViewModel
    {
        public int Id { get; set; }
        [DisplayName("Adı"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Soyadı"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı Adı"), StringLength(50)]
        public string Username { get; set; }
    }
}
