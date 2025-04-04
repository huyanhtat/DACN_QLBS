using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;

namespace DAL_QLBS
{
    public class PublisherDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();

        public PublisherDAL()
        {
            
        }

        // Tạo thêm nhà cung cấp mới
        public bool AddPublisher(publisher publisher)
        {
            try
            {
                qlbs.publishers.InsertOnSubmit(publisher);
                qlbs.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy ra danh sách nhà cung cấp
        public List<publisher> GetAllPublishers()
        {
            return qlbs.publishers.ToList();
        }

        // Cập nhật danh sách nhà cung cấp
        public bool UpdatePublisher(publisher updatedPublisher)
        {
            try
            {
                var publisher = qlbs.publishers.FirstOrDefault(p => p.id == updatedPublisher.id);
                if (publisher != null)
                {
                    publisher.name = updatedPublisher.name;
                    publisher.address = updatedPublisher.address;
                    publisher.phone = updatedPublisher.phone;
                    publisher.email = updatedPublisher.email;
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Xoá nhà cung cấp
        public bool DeletePublisher(int id)
        {
            try
            {
                var publisher = qlbs.publishers.FirstOrDefault(p => p.id == id);
                if (publisher != null)
                {
                    qlbs.publishers.DeleteOnSubmit(publisher);
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        // Lấy nhà cung cấp theo id
        public string GetPublisherNameById(int publisherId)
        {
            var publisher = qlbs.publishers.FirstOrDefault(p => p.id == publisherId);
            return publisher != null ? publisher.name : string.Empty;
        }
    }
}
