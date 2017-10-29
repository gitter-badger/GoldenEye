﻿using System;
using System.Threading;
using System.Threading.Tasks;
using GoldenEye.Backend.Core.DDD.Events;
using GoldenEye.Backend.Core.DDD.Events.Store;
using Marten;
using GoldenEye.Shared.Core.Objects.General;
using System.Linq;
using System.Collections.Generic;
using MartenEvents = Marten.Events;

namespace GoldenEye.Backend.Core.Marten.Events.Storage
{
    public class MartenEventStore : IEventStore
    {
        private readonly IDocumentSession documentSession;

        public IEventProjectionStore Projections { get; }

        public MartenEventStore(IDocumentSession documentSession)
        {
            this.documentSession = documentSession ?? throw new ArgumentException(nameof(documentSession));
            Projections = new EventProjectionStore(documentSession);
        }

        public void SaveChanges()
        {
            documentSession.SaveChanges();
        }

        public Task SaveChangesAsync(CancellationToken token = default(CancellationToken))
        {
            return documentSession.SaveChangesAsync(token);
        }

        public Guid Store(Guid stream, params IEvent[] events)
        {
            return documentSession.Events.Append(stream, events.Cast<object>().ToArray()).Id;
        }

        public Guid Store(Guid stream, int version, params IEvent[] events)
        {
            return documentSession.Events.Append(stream, version, events.Cast<object>().ToArray()).Id;
        }

        public Task<Guid> StoreAsync(Guid stream, params IEvent[] events)
        {
            return Task.FromResult(Store(stream, events));
        }

        public Task<Guid> StoreAsync(Guid stream, int version, params IEvent[] events)
        {
            return Task.FromResult(Store(stream, version, events));
        }

        public TEntity Aggregate<TEntity>(Guid streamId, int version = 0, DateTime? timestamp = null) where TEntity : class, new()
        {
            return documentSession.Events.AggregateStream<TEntity>(streamId, version, timestamp);
        }

        public Task<TEntity> AggregateAsync<TEntity>(Guid streamId, int version = 0, DateTime? timestamp = null) where TEntity : class, new()
        {
            return documentSession.Events.AggregateStreamAsync<TEntity>(streamId, version, timestamp);
        }

        public TEvent GetById<TEvent>(Guid id)
            where TEvent : class, IEvent, IHasGuidId
        {
            return documentSession.Events.Load<TEvent>(id)?.Data;
        }

        public async Task<TEvent> GetByIdAsync<TEvent>(Guid id)
            where TEvent : class, IEvent, IHasGuidId
        {
            return (await documentSession.Events.LoadAsync<TEvent>(id))?.Data;
        }

        public IList<IEvent> Query(Guid? streamId = null, int? version = null, DateTime? timestamp = null)
        {
            return Filter(streamId, version, timestamp)
                .ToList()
                .Select(ev => ev.Data)
                .OfType<IEvent>()
                .ToList();
        }

        public async Task<IList<IEvent>> QueryAsync(Guid? streamId = null, int? version = null, DateTime? timestamp = null)
        {
            return (await Filter(streamId, version, timestamp)
                 .ToListAsync())
                 .Select(ev => ev.Data)
                 .OfType<IEvent>()
                 .ToList();
        }

        public IList<TEvent> Query<TEvent>(Guid? streamId = null, int? version = null, DateTime? timestamp = null) where TEvent : class, IEvent
        {
            return Query(streamId, version, timestamp)
                .OfType<TEvent>()
                .ToList();
        }

        public async Task<IList<TEvent>> QueryAsync<TEvent>(Guid? streamId = null, int? version = null, DateTime? timestamp = null) where TEvent : class, IEvent
        {
            return (await QueryAsync(streamId, version, timestamp))
                .OfType<TEvent>()
                .ToList();
        }

        private IQueryable<MartenEvents.IEvent> Filter(Guid? streamId, int? version, DateTime? timestamp)
        {
            var query = documentSession.Events.QueryAllRawEvents().AsQueryable();

            if (streamId.HasValue)
                query = query.Where(ev => ev.StreamId == streamId);

            if (version.HasValue)
                query = query.Where(ev => ev.Version >= version);

            if (timestamp.HasValue)
                query = query.Where(ev => ev.Timestamp >= timestamp);

            return query;
        }

        public class EventProjectionStore : IEventProjectionStore
        {
            private readonly IDocumentSession documentSession;

            public EventProjectionStore(IDocumentSession documentSession)
            {
                this.documentSession = documentSession ?? throw new ArgumentException(nameof(documentSession));
            }

            public IQueryable<TProjection> Query<TProjection>()
            {
                return documentSession.Query<TProjection>();
            }

            TProjection IEventProjectionStore.GetById<TProjection>(Guid id)
            {
                return Query<TProjection>()
                    .SingleOrDefault(p => p.Id == id);
            }

            Task<TProjection> IEventProjectionStore.GetByIdAsync<TProjection>(Guid id)
            {
                return Query<TProjection>()
                    .SingleOrDefaultAsync(p => p.Id == id);
            }
        }
    }
}