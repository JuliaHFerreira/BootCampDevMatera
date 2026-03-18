using BootCampDevMatera.Views.OrderItens;

namespace BootCampDevMatera.Views.Orders
{
    public class OrderFormViewModel
    {
        public int Id { get; set; }
        public DateTime DateOrder { get; set; } = DateTime.Today;
        public int IdClient { get; set; }
        public int IdSeller { get; set; }
        public Decimal Value { get; set; }

        public List<OrderItenFormViewModel> Itens { get; set; } = new();
    }
}
