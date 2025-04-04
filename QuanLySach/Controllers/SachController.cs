using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Text.RegularExpressions;

namespace QuanLySach.Controllers
{
    public class SachController : Controller
    {
        BookManagementDataContext db = new BookManagementDataContext();


        // GET: Sach
        public ActionResult TimKiemSach( string searchString, string sortOrder, int? page, string categoryId)
        {
            var books = from s in db.books
                        join c in db.book_categories on s.code_category equals c.code
                        join j in db.book_join_authors on s.id equals j.id_book
                        join a in db.authors on j.id_author equals a.id
                        where s.priority != 4 && s.status == 1
                        select s;

            var categories = db.book_categories.ToList();
            ViewBag.Categories = new SelectList(categories, "code", "name");
            ViewBag.SelectedCategory = categoryId;

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                books = books.Where(b =>
                    b.name.ToLower().Contains(searchString) ||
                    b.book_category.name.ToLower().Contains(searchString) ||
                    b.book_join_authors.Any(j => j.author.name.ToLower().Contains(searchString))
                );
            }
            if (!String.IsNullOrEmpty(categoryId))
            {
                books = books.Where(b => b.code_category == categoryId);
            }


            switch (sortOrder)
            {
                case "asc":
                    books = books.OrderBy(b => b.price); 
                    break;
                case "desc":
                    books = books.OrderByDescending(b => b.price); 
                    break;
                default:
                    books = books.OrderBy(b => b.name);
                    break;
            }

            ViewBag.Keyword = searchString;
            ViewBag.TotalResults = books.Count(); 
            ViewBag.SortOrder = sortOrder;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(books.ToPagedList(pageNumber, pageSize)); 
        }

        [HttpGet]
        public JsonResult SearchSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var suggestions = db.books
                                .Where(b => b.name.Contains(term) && b.status == 1 && b.priority != 4)
                                .Select(b => new
                                {
                                    id = Url.Action("XemChiTiet", "Sach", new { ms = b.id }),
                                    name = b.name,
                                    image = Url.Action("GetImage", "Sach", new { id = b.id }) // Tạo URL đầy đủ
                                })
                                .Take(5)
                                .ToList();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Book_Card_More(int id)
        {
            return View(db.books.FirstOrDefault(linq => linq.id == id));
        }

        public ActionResult ComingSoon(int id)
        {
            var books = (from s in db.books
                        join c in db.book_categories on s.code_category equals c.code
                        where s.id == id && s.priority == 4 && s.status == 1
                        select new ChiTietSach{ Id = id, chuDe = c.name, tenSach = s.name  }).FirstOrDefault();
            return View(books);
        }

        public ActionResult FeaturedBook()
        {
            var book = db.books.Where(t => t.priority == 1 && t.priority != 4 && t.status == 1).OrderByDescending(t=>t.id).ToList();
            return View(book.Take(4));
        }

        public ActionResult NewBook()
        {
            var book = db.books.Where(t => t.priority == 2 && t.priority != 4 && t.status == 1).OrderByDescending(t=>t.id).ToList();
            return View(book.Take(4));
        }

        private string FormatMoTa(string description)
        {
            string decodedText = WebUtility.HtmlDecode(description);
            string formattedText = Regex.Replace(decodedText, @"\.|\:", ".<br>");
            return formattedText;
        }

        public ActionResult GetImage(int id)
        {
            var book = db.books.FirstOrDefault(b => b.id == id);
            if (book != null && book.image != null) 
            {
                return File(book.image.ToArray(), "image/jpeg"); 
            }
            return HttpNotFound(); 
        }

        public ActionResult GetImageAuthor(int id)
        {
            var authorImage = (from s in db.books
                               join b in db.book_join_authors on s.id equals b.id_book
                               join a in db.authors on b.id_author equals a.id
                               where s.id == id
                               select a.image).FirstOrDefault(); // Truy xuất ảnh đầu tiên

            if (authorImage != null)
            {
                return File(authorImage.ToArray(), "image/jpeg");
            }
            return HttpNotFound();
        }

        public ActionResult XemChiTiet(int ms)
        {
            var sp = (from s in db.books
                      join c in db.book_categories on s.code_category equals c.code
                      join j in db.book_join_authors on s.id equals j.id_book
                      join a in db.authors on j.id_author equals a.id
                      join p in db.publishers on s.id_publisher equals p.id
                      where s.id == ms
                      select new ChiTietSach
                      {
                          Id = ms,
                          tenSach = s.name,
                          gia = (float)s.price,
                          moTa = FormatMoTa(s.description),
                          tenTacGia = a.name,
                          chuDe = c.name,
                          NXB = p.name,
                          soLuong = s.quantity, 
                          number_of_views = s.number_of_views ?? 0 + 1,
                          number_of_purchases = s.number_of_purchases ?? 0,
                          bio = a.bio
                      }).FirstOrDefault();

            if (sp == null)
            {
                return HttpNotFound();
            }      
            
            var bookToUpdate = db.books.FirstOrDefault(b => b.id == ms);
            if (bookToUpdate != null)
            {
                bookToUpdate.number_of_views += 1;
                db.SubmitChanges();
            }
            return View(sp);
        }

        public ActionResult TatCaSach(int? page)
        {
            var categories = db.book_categories.ToList();
            ViewBag.Categories = new SelectList(categories, "code", "name");
            if (page == null) page = 1;
            var links = (from l in db.books
                         where l.priority != 4 && l.status != 0
                         select l).OrderByDescending(x => x.id);
            int pageSize = 12;
            int pageNumber = (page ?? 1);

            return View(links.ToPagedList(pageNumber, pageSize));
        }


        //K-Mean

        public ActionResult Details(int productId)
        {
            KMeans k = new KMeans();
            // 1. Lấy dữ liệu sản phẩm từ cơ sở dữ liệu
            var products = GetProductsFromDatabase();

            // 2. Huấn luyện model K-Means
            var model = k.TrainModel(products, numberOfClusters: 1);

            // 3. Phân cụm các sản phẩm
            var clusters = k.PredictClusters(model, products);

            // 4. Gán cụm vào danh sách sản phẩm
            for (int i = 0; i < products.Count; i++)
            {
                products[i].ClusterId = (int)clusters[i].Cluster;
            }

            // 5. Lấy sản phẩm hiện tại và các sản phẩm tương tự trong cùng cụm
            var currentProduct = products.FirstOrDefault(p => p.Id == productId);
            var similarProducts = products
                .Where(p => p.ClusterId == currentProduct.ClusterId && p.Id != productId)
                .ToList();

            // 6. Truyền dữ liệu ra View
            ViewBag.CurrentProduct = currentProduct;
            return View(similarProducts.Take(4));
        }

        private List<ChiTietSach> GetProductsFromDatabase()
        {
            var books = (from s in db.books
                        join c in db.book_categories on s.code_category equals c.code
                        join j in db.book_join_authors on s.id equals j.id_book
                        join a in db.authors on j.id_author equals a.id
                        where s.priority != 4 && s.status != 0
                        select new ChiTietSach
                        {
                            Id = s.id,
                            gia = (float)s.price,
                            tenSach = s.name,
                            number_of_views = s.number_of_views.HasValue ? (float)s.number_of_views.Value : 0f, 
                            number_of_purchases = s.number_of_purchases.HasValue ? (float)s.number_of_purchases.Value : 0f, 
                            chuDe = c.name,
                            soLuong = s.quantity
                        }).ToList();
            return books;

        }
    }
}