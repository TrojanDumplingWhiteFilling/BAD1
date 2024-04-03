using CsvHelper.Configuration.Attributes;

using System;

namespace Bakery.Models.Csv
{
    public class BakingRecordBase
    {
        // For Orders
        [Name("Order_DeliveryPlace")]
        public string DeliveryPlace { get; set; }

        [Name("Order_DeliveryDate")]
        public DateTime DeliveryDate { get; set; }

        // For BakingGoods
        [Name("BakingGood_BgName")]
        public string BgName { get; set; }

        [Name("BakingGood_DateProduced")]
        public DateTime DateProduced { get; set; }

        // For Batches
        [Name("Batch_StartDateTime")]
        public DateTime BatchStartDateTime { get; set; }

        [Name("Batch_EndDateTime")]
        public DateTime BatchEndDateTime { get; set; }

        [Name("Batch_ActualEndTime")]
        public DateTime BatchActualEndTime { get; set; }

        // For Stock
        [Name("Stock_SkName")]
        public string SkName { get; set; }

        [Name("Stock_Quantity")]
        public int StockQuantity { get; set; }
    }

    public class BakingRecordMap
    {
        // For Packets
        [Name("Packet_TrackId")]
        public string TrackId { get; set; }

        [Name("Packet_OrderId")]
        public int PacketOrderId { get; set; }

        // For OrderBakingGood
        [Name("OrderBakingGood_OrderId")]
        public int OrderBakingGoodOrderId { get; set; }

        [Name("OrderBakingGood_BgName")]
        public int OrderBakingGood_BgName { get; set; }

        [Name("OrderBakingGood_Quantity")]
        public int OrderBakingGoodQuantity { get; set; }

        // for BakingGoodBatch
        [Name("BakingGoodBatch_BgName")]
        public int BakingGoodBatch_BgName { get; set; }

        [Name("BakingGoodBatch_BatchId")]
        public int BakingGoodBatchBatchId { get; set; }

        [Name("BakingGoodBatch_Quantity")]
        public int BakingGoodBatchQuantity { get; set; }

        // For BatchStock
        [Name("BatchStock_BatchId")]
        public int BatchStockBatchId { get; set; }

        [Name("BatchStock_StockId")]
        public int BatchStockStockId { get; set; }

        [Name("BatchStock_Quantity")]
        public int BatchStockQuantity { get; set; }
    }
}