using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class PublisherBLL
    {
        private PublisherDAL publisherDAL;
        public PublisherBLL()
        {
            publisherDAL = new PublisherDAL();
        }
        public bool AddPublisher(publisher publisher)
        {
            return publisherDAL.AddPublisher(publisher);
        }

        public List<publisher> GetAllPublishers()
        {
            return publisherDAL.GetAllPublishers();
        }

        public bool UpdatePublisher(publisher publisher)
        {
            return publisherDAL.UpdatePublisher(publisher);
        }

        public bool DeletePublisher(int id)
        {
            return publisherDAL.DeletePublisher(id);
        }
    }
}
