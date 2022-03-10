using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Stateless;

namespace FakeTravel.API.Models
{
    public enum OrderStatuEnum
    {
        Peding,//订单已生成
        Processing,//支付处理中
        Completed,//交易成功
        Decliend,//交易失败
        Cancelled,//订单取消
        Refund,//已退款
    }
    public enum OrderStatuTriggerEnum
    {
        PlaceOrder,//支付
        Approve,//支付成功
        Reject,//支付失败
        Cancel,//取消
        Return,//退货
    }
    public class Order
    {
        public Order()
        {
            StateMachineInit();
        }

        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStatuEnum State { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadate { get; set; }
        StateMachine<OrderStatuEnum, OrderStatuTriggerEnum> _machine;
        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStatuEnum, OrderStatuTriggerEnum>(
                OrderStatuEnum.Peding);

            _machine.Configure(OrderStatuEnum.Peding)
                .Permit(OrderStatuTriggerEnum.PlaceOrder, OrderStatuEnum.Processing)
                .Permit(OrderStatuTriggerEnum.Cancel, OrderStatuEnum.Cancelled);

            _machine.Configure(OrderStatuEnum.Processing)
                .Permit(OrderStatuTriggerEnum.Approve, OrderStatuEnum.Completed)
                .Permit(OrderStatuTriggerEnum.Reject, OrderStatuEnum.Decliend);

            _machine.Configure(OrderStatuEnum.Decliend)
                .Permit(OrderStatuTriggerEnum.PlaceOrder, OrderStatuEnum.Processing);

            _machine.Configure(OrderStatuEnum.Completed)
                .Permit(OrderStatuTriggerEnum.Return, OrderStatuEnum.Refund);
        }
    }
}
