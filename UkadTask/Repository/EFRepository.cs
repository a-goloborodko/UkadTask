using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using UkadTask.Domain;

namespace UkadTask.Repository
{
    public class EFRepository : IRepository
    {
        private EFContext _context;
        private bool _disposed;

        public EFRepository()
        {
            _context = new EFContext();
        }

        #region IRepository
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void AddHost(Host host)
        {
            _context.Entry(host).State = EntityState.Added;
        }

        public void AddPage(Page page)
        {
            _context.Entry(page).State = EntityState.Added;
        }

        public void UpdateHost(Host host)
        {
            Host findedHost = _context.Hosts.Find(host.Id);
            if (findedHost != null)
                _context.Entry(host).State = EntityState.Modified;
        }

        public void UpdatePage(Page page)
        {
            Page findedPage = _context.Pages.Find(page.Id);
            if (findedPage != null)
                _context.Entry(page).State = EntityState.Modified;
        }

        public void AddHistory(History history)
        {
            _context.Entry(history).State = EntityState.Added;
        }

        public void AddRangePage(IEnumerable<Page> pages)
        {
            _context.Pages.AddRange(pages);
        }

        public void AddRangeHistory(IEnumerable<History> history)
        {
            _context.History.AddRange(history);
        }

        public IEnumerable<Host> GetHosts()
        {
            return _context.Hosts.ToList();
        }

        public IEnumerable<Page> GetPages()
        {
            return _context.Pages.ToList();
        }

        public IEnumerable<Host> GetHostsWOTracking()
        {
            return _context.Hosts.AsNoTracking().ToList();
        }
        public IEnumerable<Page> GetPagesWOTracking()
        {
            return _context.Pages.AsNoTracking().ToList();
        }

        public IEnumerable<Page> GetPagesForHost(int hostId)
        {
            return _context.Pages.Where(x => x.HostId == hostId).ToList();
        }

        public IEnumerable<Page> GetPagesForHost(string hostUrl)
        {
            return _context.Pages
                .Include(x => x.Host)
                .Where(x => x.Host.HostName == hostUrl)
                .ToList();
        }

        public Host GetHostById(int id)
        {
            return _context.Hosts.Find(id);
        }

        public Host GetHostByUrl(string hostUrl)
        {
            return _context.Hosts.SingleOrDefault(x => x.HostName == hostUrl);
        }

        public Host GetHostByUrlIncludPages(string hostUrl)
        {
            return _context.Hosts
                .Include(x => x.Pages)
                .SingleOrDefault(x => x.HostName == hostUrl);
        }

        public IEnumerable<History> GetHistoryForHost(int hostId)
        {
            return _context.History
                 .Include(x => x.Page)
                 .Where(x => x.Page.HostId == hostId)
                 .AsNoTracking()
                 .ToList();
        }

        public IEnumerable<History> GetHistoryForHost(string hostUrl)
        {
            return _context.History
                .Include(x => x.Page)
                .Include(x => x.Page.Host)
                .Where(x => x.Page.Host.HostName == hostUrl)
                .AsNoTracking()
                .ToList();
        }

        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
