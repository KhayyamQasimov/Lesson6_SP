using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ThreadManager
{
    private SemaphoreSlim semaphore;
    private object lockObj = new object();
    private int nextId = 1;

    public int Places { get; private set; }
    public List<ThreadItem> Created = new List<ThreadItem>();
    public List<ThreadItem> Waiting = new List<ThreadItem>();
    public List<ThreadItem> Working = new List<ThreadItem>();

    public ThreadManager(int places)
    {
        Places = places;
        semaphore = new SemaphoreSlim(places);
    }

    public void Create()
    {
        lock (lockObj)
        {
            Created.Add(new ThreadItem { Id = nextId++ });
        }
    }

    public void SendToWaiting(int id)
    {
        ThreadItem item;
        lock (lockObj)
        {
            item = Created.FirstOrDefault(t => t.Id == id);
            if (item == null) return;
            Created.Remove(item);
            Waiting.Add(item);
        }
        new Thread(() => Work(item)).Start();
    }

    private void Work(ThreadItem item)
    {
        semaphore.Wait(); 

        lock (lockObj)
        {
            Waiting.Remove(item);
            Working.Add(item);
        }

        item.Remaining = 10;
        while (item.Remaining > 0 && !item.Released)
        {
            Thread.Sleep(1000);
            item.Remaining--;
        }

        lock (lockObj)
        {
            Working.Remove(item);
        }

        semaphore.Release();
    }

    public void Release(int id)
    {
        lock (lockObj)
        {
            var item = Working.FirstOrDefault(t => t.Id == id);
            if (item != null) item.Released = true;
        }
    }

    public void IncreasePlaces()
    {
        Places++;
        semaphore.Release();
    }

    public void DecreasePlaces()
    {
        if (Places <= 1) return;
        Places--;
        Task.Run(() => semaphore.Wait());
    }
    public List<ThreadItem> GetWorking() { lock (lockObj) return Working.ToList(); }
    public List<ThreadItem> GetWaiting() { lock (lockObj) return Waiting.ToList(); }
    public List<ThreadItem> GetCreated() { lock (lockObj) return Created.ToList(); }
}
