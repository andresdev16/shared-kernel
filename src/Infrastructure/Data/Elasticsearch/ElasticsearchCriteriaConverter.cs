//using System;
//using System.Collections.Generic;

//namespace SharedKernel.Infrastructure.Data.Elasticsearch
//{
//    public class ElasticsearchCriteriaConverter<T> where T : class
//    {
//        private readonly Dictionary<FilterOperator, Func<Filter, QueryContainer>> _queryTransformers =
//            new Dictionary<FilterOperator, Func<Filter, QueryContainer>>
//            {
//                {FilterOperator.EQUAL, TermQuery},
//                {FilterOperator.NOT_EQUAL, TermQuery},
//                {FilterOperator.GT, GreaterThanQueryTransformer},
//                {FilterOperator.GTE, GreaterThanOrEqualQueryTransformer},
//                {FilterOperator.LT, LessThanQueryTransformer},
//                {FilterOperator.LTE, LessThanOrEqualQueryTransformer},
//                {FilterOperator.CONTAINS, WildcardTransformer},
//                {FilterOperator.NOT_CONTAINS, WildcardTransformer}
//            };

//        public SearchDescriptor<T> Convert(Criteria criteria, string index)
//        {
//            if (criteria == null) return new SearchDescriptor<T>();

//            var searchDescriptor = new SearchDescriptor<T>();

//            searchDescriptor.From(criteria.Offset ?? 0);
//            searchDescriptor.Size(criteria.Limit ?? 1000);

//            if (criteria.HasFilters()) searchDescriptor.Query(q => QueryByCriteria(criteria));
//            if (criteria.HasOrder()) searchDescriptor.Sort(s => CreateSortDescriptor(s, criteria));
//            searchDescriptor.Index(index);

//            return searchDescriptor;
//        }

//        private SortDescriptor<T> CreateSortDescriptor(SortDescriptor<T> sortDescriptor, Criteria criteria)
//        {
//            if (!criteria.HasOrder())
//                return null;

//            var orderByValue = criteria.Order.OrderBy.Value;
//            var sortOrder = criteria.Order.OrderType == OrderType.ASC ? SortOrder.Ascending : SortOrder.Descending;

//            return sortDescriptor.Field(f => f.Field(orderByValue).Order(sortOrder));
//        }

//        private QueryContainer QueryByCriteria(Criteria criteria)
//        {
//            if (!criteria.HasFilters())
//                return null;

//            QueryContainer query = null;

//            foreach (var filter in criteria.Filters.Values)
//            {
//                var element = _queryTransformers[filter.Operator];
//                query &= filter.Operator.IsPositive() ? element(filter) : !element(filter);
//            }

//            return query;
//        }

//        private static QueryContainer TermQuery(Filter filter)
//        {
//            return Query<T>.Term(filter.Field.Value, filter.Value.Value.ToLower());
//        }

//        private static QueryContainer GreaterThanQueryTransformer(Filter filter)
//        {
//            return Query<T>.Range(r => r.Field(filter.Field.Value).GreaterThan(Double.Parse(filter.Value.Value)));
//        }

//        private static QueryContainer GreaterThanOrEqualQueryTransformer(Filter filter)
//        {
//            return Query<T>.Range(
//                r => r.Field(filter.Field.Value).GreaterThanOrEquals(Double.Parse(filter.Value.Value)));
//        }

//        private static QueryContainer LessThanQueryTransformer(Filter filter)
//        {
//            return Query<T>.Range(r => r.Field(filter.Field.Value).LessThan(Double.Parse(filter.Value.Value)));
//        }

//        private static QueryContainer LessThanOrEqualQueryTransformer(Filter filter)
//        {
//            return Query<T>.Range(r => r.Field(filter.Field.Value).LessThanOrEquals(Double.Parse(filter.Value.Value)));
//        }

//        private static QueryContainer WildcardTransformer(Filter filter)
//        {
//            return Query<T>.Wildcard(filter.Field.Value, $"*{filter.Value.Value}*");
//        }
//    }
//}
