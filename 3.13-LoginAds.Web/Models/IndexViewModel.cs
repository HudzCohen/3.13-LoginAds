using _3._13_LoginAds.Data;

namespace _3._13_LoginAds.Web.Models
{
    public class IndexViewModel
    {
        public List<Ad> Ads { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
    }
}
