using Microsoft.ML;
using Microsoft.ML.Data;
using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Controllers
{
    public class KMeans
    {
        private readonly MLContext _mlContext;

        public KMeans()
        {
            _mlContext = new MLContext();
        }

        public ITransformer TrainModel(List<ChiTietSach> products, int numberOfClusters)
        {
            // Load dữ liệu
            var data = _mlContext.Data.LoadFromEnumerable(products);

            // Pipeline: Chuẩn bị dữ liệu và huấn luyện K-Means
            var pipeline = _mlContext.Transforms
               .Conversion.ConvertType("number_of_views", outputKind: DataKind.Single) // Chuyển thành float
               .Append(_mlContext.Transforms.Conversion.ConvertType("number_of_purchases", outputKind: DataKind.Single)) // Chuyển thành float
               .Append(_mlContext.Transforms.Concatenate("Features", nameof(ChiTietSach.gia), "number_of_views", "number_of_purchases"))
               .Append(_mlContext.Clustering.Trainers.KMeans(numberOfClusters: numberOfClusters));

            // Huấn luyện model
            var model = pipeline.Fit(data);

            return model;
        }

        public List<ProductCluster> PredictClusters(ITransformer model, List<ChiTietSach> products)
        {
            // Load dữ liệu
            var data = _mlContext.Data.LoadFromEnumerable(products);

            // Biến đổi dữ liệu với model
            var transformedData = model.Transform(data);

            // Trích xuất dự đoán
            var predictions = _mlContext.Data.CreateEnumerable<ProductCluster>(transformedData, reuseRowObject: false).ToList();

            return predictions;
        }
    }
}