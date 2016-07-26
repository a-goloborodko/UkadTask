using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UkadTask.Domain;

namespace UkadTask.Repository
{
    public interface IRepository : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();

        void AddHost(Host host);
        void AddPage(Page page);
        void UpdateHost(Host host);
        void UpdatePage(Page page);
        void AddHistory(History history);
        void AddRangePage(IEnumerable<Page> pages);
        void AddRangeHistory(IEnumerable<History> history);
        IEnumerable<Host> GetHosts();
        IEnumerable<Host> GetHostsWOTracking();
        IEnumerable<Page> GetPages();
        IEnumerable<Page> GetPagesWOTracking();
        IEnumerable<Page> GetPagesForHost(int hostId);
        IEnumerable<Page> GetPagesForHost(string hostUrl);
        Host GetHostById(int id);
        Host GetHostByUrl(string hostUrl);
        Host GetHostByUrlIncludPages(string hostUrl);
        IEnumerable<History> GetHistoryForHost(int hostId);
        IEnumerable<History> GetHistoryForHost(string hostUrl);
    }
}
