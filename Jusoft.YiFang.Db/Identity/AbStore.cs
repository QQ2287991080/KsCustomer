using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer.Utilities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.Identity
{

    public abstract class AbStore : IDisposable
    {
        protected DbContext Context { get; private set; }
        public bool DisposeContext { get; set; }
        public bool AutoSaveChanges { get; set; }

        protected bool _disposed;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public AbStore(DbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException("context");
            this.AutoSaveChanges = true;
        }
        protected void ThrowIfDisposed()
        {
            if(this._disposed) {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }
        protected async Task SaveChanges()
        {
            if(!this.AutoSaveChanges) {
                return;
            }
            await this.Context.SaveChangesAsync().WithCurrentCulture();
        }

        protected virtual void Dispose(bool disposing)
        {
            if((this.DisposeContext && disposing) && (this.Context != null)) {
                this.Context.Dispose();
            }
            this._disposed = true;
            this.Context = null;
        }

    }
}
