using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net.Cache;
using System.Linq;
using IFood.Entities;
using IFood.Enumerators;

namespace IFood
{
    public class Api
    {
        #region helper
        private enum HttpMethod
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE
        }

        private enum UrlType
        {
            authentication,
            merchant,
            catalog,
            order,
        }

        private class IFoodToken
        {
            private string _accessToken;
            private string _type;
            private int _expiresIn;
            private DateTime _createdDate;

            [JsonProperty(PropertyName = "accessToken")]
            public string AccessToken
            {
                get { return _accessToken; }
                set { _accessToken = value; }
            }

            [JsonProperty(PropertyName = "type")]
            public string Type
            {
                get { return _type; }
                set { _type = value; }
            }

            [JsonProperty(PropertyName = "expiresIn")]
            public int ExpiresIn
            {
                get { return _expiresIn; }
                set { _expiresIn = value; }
            }

            [JsonIgnore]
            public DateTime CreatedDate
            {
                get { return _createdDate; }
                set { _createdDate = value; }
            }

            [JsonIgnore]
            public bool IsValid
            {
                get { return _createdDate.AddSeconds(_expiresIn - 60) > DateTime.Now; }
            }
        }
        #endregion

        #region private static members

        #region static config
        private const int _productsPerPage = 100;
        #endregion

        #region endpoints
        private const string _baseUrlFormat = "https://merchant-api.ifood.com.br/{0}/v1.0";
        private const string _tokenUrl = "/oauth/token";
        private const string _merchantsUrl = "/merchants";
        private const string _merchantUrlFormat = "/merchants/{0}";
        private const string _merchantStatusUrlFormat = "/merchants/{0}/status";
        private const string _merchantDeliveryStatusUrlFormat = "/merchants/{0}/status/delivery";
        private const string _merchantInterruptionsUrlFormat = "/merchants/{0}/interruptions";
        private const string _eventUrl = "/events:polling";
        private const string _acknowledgedUrl = "/events/acknowledgment";
        private const string _orderUrlFormat = "/orders/{0}";
        private const string _orderConfirmUrlFormat = "/orders/{0}/confirm";
        private const string _orderStartPreparationUrlFormat = "/orders/{0}/startPreparation";
        private const string _orderReadyToPickupUrlFormat = "/orders/{0}/readyToPickup";
        private const string _orderDispatchUrlFormat = "/orders/{0}/dispatch";
        private const string _orderRequestCancellationUrlFormat = "/orders/{0}/requestCancellation";
        private const string _orderAcceptCancellationUrlFormat = "/orders/{0}/acceptCancellation";
        private const string _orderDenyCancellationUrlFormat = "/orders/{0}/denyCancellation";
        private const string _productUrlFormat = "/merchants/{0}/products";
        private const string _catalogsUrlFormat = "/merchants/{0}/catalogs";
        private const string _catalogUrlFormat = "/merchants/{0}/catalogs/{1}/categories";
        private const string _categoryItemUrlFormat = "/merchants/{0}/categories/{1}/products/{2}";
        private const string _itemUrlFormat = "/merchants/{0}/items/{1}";
        private const string _optionGroupUrlFormat = "/merchants/{0}/optionGroups";
        private const string _optionGroupProductUrlFormat = "/merchants/{0}/optionGroups/{1}/products/{2}";
        private const string _optionProductUrlFormat = "/merchants/{0}/optionGroups/{1}/products/{2}/option";
        private const string _pizzasUrlFormat = "/merchants/{0}/pizzas";
        private const string _pizzaCategoryUrlFormat = "/merchants/{0}/pizzas/{1}/categories/{2}";
        #endregion

        #endregion

        #region private members
        private string _client_id = "92caaaca-3f65-44f0-bbc8-7df3a1b3228c";
        private string _client_secret = "ufheozhfqt7zi6n33cjpgrzh21x62pvao2enynbb3dceu8vridqdd5o4t1n8ftbsybjw3s8yh34g9bzefuxr1ftllklg16vfb3s";
        private string _merchantId = "6e66bdac-0e2a-42d0-b621-74311cbba57e";
        private IFoodToken _token;
        #endregion

        #region public methods

        #region manager
        public bool Authenticate() => GetToken();
        #endregion

        #region merchant
        //not in use
        public void GetMerchants()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.merchant, _merchantsUrl));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var definition = new[]
                    {
                        new
                        {
                            id = "",
                            name = "",
                            corporateName = ""
                        }
                    };
                    var result = JsonConvert.DeserializeAnonymousType(response, definition);
                }
            }
        }
        //not in use
        public void GetMerchantDetails()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.merchant, string.Format(_merchantUrlFormat, _merchantId)));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var definition = new
                    {
                        id = "",
                        name = "",
                        corporateName = "",
                        description = "",
                        averageTicket = 0,
                        exclusive = false,
                        status = IFoodAvailabilityStatus.UNAVAILABLE,
                        createdAt = DateTime.MinValue,
                        address = new
                        {
                            country = "",
                            state = "",
                            city = "",
                            district = "",
                            street = "",
                            number = "",
                            postalCode = "",
                            latitude = 0.0f,
                            longitude = 0.0f
                        },
                        operations = new[]
                        {
                            new
                            {
                                name = "",
                                salesChannels= new []
                                {
                                    new
                                    {
                                        name= "",
                                        enabled= false
                                    }
                                }
                            }
                        }
                    };
                    var result = JsonConvert.DeserializeAnonymousType(response, definition);
                }
            }
        }
        public bool? GetMerchantStatus()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.merchant, string.Format(_merchantStatusUrlFormat, _merchantId)));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var definition = new[]
                    {
                        new
                        {
                            operation = "",
                            salesChannel = "",
                            available = false,
                            state = "",
                            reopenable = new
                            {
                                identifier = "",
                                type = "",
                                reopenable = false
                            },
                            validations = new[]
                            {
                                new
                                {
                                    id = "",
                                    code = "",
                                    state = "",
                                    message = new
                                    {
                                        title = "",
                                        subtitle = "",
                                        description = "",
                                        priority = 0,
                                    }
                                }
                            },
                            message = new
                            {
                                title = "",
                                subtitle = "",
                                description = "",
                                priority = "",
                            }
                        }
                    };
                    var result = JsonConvert.DeserializeAnonymousType(response, definition);
                    if (result != null && result.Length > 0)
                        return result[0].available;
                }
            }
            return null;
        }
        //not in use
        public void GetMerchantDeliveryStatus()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.merchant, string.Format(_merchantDeliveryStatusUrlFormat, _merchantId)));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var definition = new
                    {
                        operation = "",
                        salesChannel = "",
                        available = false,
                        state = "",
                        reopenable = new
                        {
                            identifier = "",
                            type = "",
                            reopenable = false
                        },
                        validations = new[]
                        {
                            new
                            {
                                id = "",
                                code = "",
                                state = "",
                                message = new
                                {
                                    title = "",
                                    subtitle = "",
                                    description = "",
                                    priority = 0,
                                }
                            }
                        },
                        message = new
                        {
                            title = "",
                            subtitle = "",
                            description = "",
                            priority = "",
                        }
                    };
                    var result = JsonConvert.DeserializeAnonymousType(response, definition);
                }
            }
        }
        //not in use
        public void GetMerchantInterruptions()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.merchant, string.Format(_merchantInterruptionsUrlFormat, _merchantId)));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var definition = new[]
                    {
                        new
                        {
                            id = "",
                            description = "",
                            start = DateTime.MinValue,
                            end = DateTime.MinValue
                        }
                    };
                    var result = JsonConvert.DeserializeAnonymousType(response, definition);
                }
            }
        }
        #endregion

        #region order
        public List<IFoodOrder> GetOrders()
        {
            if (GetToken())
            {
                List<KeyValuePair<string, string>> merchants = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("x-polling-merchants",_merchantId)
                };

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.NoContent };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.order, _eventUrl), addicionalHeaders: merchants);
                if (answer.Count == 1)
                {
                    if (answer.Contains(HttpStatusCode.OK) && !string.IsNullOrEmpty(response))
                        return JsonConvert.DeserializeObject<List<IFoodOrder>>(response);
                    else if (answer.Contains(HttpStatusCode.NoContent))
                        return new List<IFoodOrder>();
                }
            }
            return null;
        }
        public bool AcknowledgedOrder(List<IFoodOrder> orders)
        {
            //em modo de teste nunca confirma que recebeu
            //if (System.Diagnostics.Debugger.IsAttached)
            //    return true;

            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Accepted };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, _acknowledgedUrl), JsonConvert.SerializeObject(orders));
                return answer.Contains(HttpStatusCode.OK) || answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        public IFoodOrderDetail GetOrderDetails(string id)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.order, string.Format(_orderUrlFormat, id)));
                if (answer.Contains(HttpStatusCode.OK) && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<IFoodOrderDetail>(response);
            }
            return null;
        }
        public bool ConfirmOrder(IFoodOrder order)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderConfirmUrlFormat, order.CorrelationId)), JsonConvert.SerializeObject(order));
                return answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        //not in use
        public void StartPreparationOrder(IFoodOrder order)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderStartPreparationUrlFormat, order.CorrelationId)), JsonConvert.SerializeObject(order));
                //return answer.Contains(HttpStatusCode.Accepted);
            }
        }
        public bool ReadyToPickupOrder(IFoodOrder order)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderReadyToPickupUrlFormat, order.CorrelationId)), JsonConvert.SerializeObject(order));
                return answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        public bool DispatchOrder(IFoodOrder order)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderDispatchUrlFormat, order.CorrelationId)), JsonConvert.SerializeObject(order));
                return answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        public bool RequestCancellationOrder(string id, string reason, string cancellationCode)
        {
            if (GetToken())
            {
                var definition = new
                {
                    reason = reason,
                    cancellationCode = cancellationCode
                };
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderRequestCancellationUrlFormat, id)), JsonConvert.SerializeObject(definition));
                return answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        public bool AcceptCancellationOrder(string id)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderAcceptCancellationUrlFormat, id)));
                return answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        public bool DenyCancellationOrder(string id)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Accepted };
                Request(ref answer, HttpMethod.POST, GetUrl(UrlType.order, string.Format(_orderDenyCancellationUrlFormat, id)));
                return answer.Contains(HttpStatusCode.Accepted);
            }
            return false;
        }
        #endregion

        #region products
        public List<IFoodProduct> GetProducts()
        {
            if (GetToken())
            {
                int page = 1;
                var definition = new
                {
                    page = 0,
                    count = 0,
                    limit = 0,
                    elements = new IFoodProduct[0]
                };

                bool canExecute;
                var products = new List<IFoodProduct>();
                do
                {
                    canExecute = false;
                    var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                    var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.catalog, string.Format(_productUrlFormat, _merchantId) + "?limit=" + _productsPerPage + "&page=" + page++));
                    if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    {
                        var result = JsonConvert.DeserializeAnonymousType(response, definition);
                        if (result != null && result.count > 0)
                        {
                            products.AddRange(result.elements);
                            canExecute = result.count == result.limit;
                        }
                    }
                }
                while (canExecute);

                return products;
            }
            return null;
        }
        public IFoodProduct InsertProduct(IFoodProduct product)
        {
            if (GetToken())
            {
                product.Id = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(product.Description))
                    product.Description = " ";

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_productUrlFormat, _merchantId)), JsonConvert.SerializeObject(product));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<IFoodProduct>(response);
            }
            return null;
        }
        public IFoodProduct UpdateProduct(IFoodProduct product)
        {
            if (GetToken())
            {
                if (string.IsNullOrEmpty(product.Description))
                    product.Description = " ";

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.PUT, GetUrl(UrlType.catalog, string.Format(_productUrlFormat, _merchantId) + "/" + product.Id), JsonConvert.SerializeObject(product));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<IFoodProduct>(response);
                    if (string.IsNullOrEmpty(result.Id))
                        result.Id = product.Id;
                    return result;
                }
            }
            return null;
        }
        public bool DeleteProduct(IFoodProduct product)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_productUrlFormat, _merchantId) + "/" + product.Id));
                return answer.Count == 1;
            }
            return false;
        }
        #endregion

        #region catalogs
        public List<string> GetCatalogs()
        {
            if (GetToken())
            {
                var definition = new[]
                {
                    new
                    {
                        catalogId = "",
                        //context = new List<string>(),
                        status = ""
                    }
                };
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.catalog, string.Format(_catalogsUrlFormat, _merchantId)));
                if (answer.Contains(HttpStatusCode.OK) && !string.IsNullOrEmpty(response))
                {
                    var catalogs = JsonConvert.DeserializeAnonymousType(response, definition);
                    if (catalogs != null)
                    {
                        var result = new List<string>();
                        foreach (var catalog in catalogs)
                            if (catalog.status == "AVAILABLE")
                                result.Add(catalog.catalogId);
                        return result.Count > 0 ? result : null;
                    }
                }
            }
            return null;
        }
        #endregion

        #region categories
        public void GetCategories(string catalogId, out List<IFoodItemCategory> itemsCategories, out List<IFoodPizzaCategory> pizzasCategories)
        {
            itemsCategories = null;
            pizzasCategories = null;
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.catalog, string.Format(_catalogUrlFormat, _merchantId, catalogId)));
                if (answer.Contains(HttpStatusCode.OK) && !string.IsNullOrEmpty(response))
                {
                    var tmpItems = JsonConvert.DeserializeObject<List<IFoodItemCategory>>(response);
                    itemsCategories = tmpItems.Where(x => x.Items != null).ToList();
                    var tmpPizzas = JsonConvert.DeserializeObject<List<IFoodPizzaCategory>>(response);
                    pizzasCategories = tmpPizzas.Where(x => x.Pizza != null).ToList();
                }
            }
        }
        public T InsertCategory<T>(T category, string catalogId) where T : IFoodCategory
        {
            if (GetToken())
            {
                category.Id = Guid.NewGuid().ToString();

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_catalogUrlFormat, _merchantId, catalogId)), JsonConvert.SerializeObject(category));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<T>(response);
                    ReAssign(category, result);
                    return result;
                }
            }
            return null;
        }
        public T UpdateCategory<T>(T category, string catalogId) where T : IFoodCategory
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.PATCH, GetUrl(UrlType.catalog, string.Format(_catalogUrlFormat, _merchantId, catalogId) + "/" + category.Id), JsonConvert.SerializeObject(category));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<T>(response);
                    ReAssign(category, result);
                    return result;
                }
            }
            return null;
        }
        public bool DeleteCategory(IFoodCategory category, string catalogId)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_catalogUrlFormat, _merchantId, catalogId) + "/" + category.Id), JsonConvert.SerializeObject(category));
                return answer.Count == 1;
            }
            return false;
        }

        public void ReAssign<T>(T category, T result) where T : IFoodCategory
        {
            if (string.IsNullOrWhiteSpace(result.Id) && !string.IsNullOrWhiteSpace(category.Id))
                result.Id = category.Id;

            switch (result)
            {
                case IFoodItemCategory x:
                    if (x.Items == null)
                        switch (category)
                        {
                            case IFoodItemCategory y:
                                x.Items = y.Items;
                                break;
                        }
                    break;

                case IFoodPizzaCategory x:
                    if (x.Pizza == null)
                        switch (category)
                        {
                            case IFoodPizzaCategory y:
                                x.Pizza = y.Pizza;
                                break;
                        }
                    //if (x.Config == null)
                    //    switch (category)
                    //    {
                    //        case IFoodPizzaCategory y:
                    //            x.Config = y.Config;
                    //            break;
                    //    }
                    break;
            }
        }
        #endregion

        #region items
        public IFoodItem GetItem(string id)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.catalog, string.Format(_itemUrlFormat, _merchantId, id)));
                if (answer.Contains(HttpStatusCode.OK) && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<IFoodItem>(response);
            }
            return null;
        }
        public IFoodItem InsertItem(IFoodItem item, string categoryId)
        {
            if (GetToken())
            {
                item.Id = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(item.Description))
                    item.Description = " ";

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_categoryItemUrlFormat, _merchantId, categoryId, item.ProductId)), JsonConvert.SerializeObject(item));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<IFoodItem>(response);
            }
            return null;
        }
        public IFoodItem UpdateItem(IFoodItem item, string categoryId)
        {
            if (GetToken())
            {
                if (string.IsNullOrEmpty(item.Description))
                    item.Description = " ";

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.PATCH, GetUrl(UrlType.catalog, string.Format(_categoryItemUrlFormat, _merchantId, categoryId, item.ProductId)), JsonConvert.SerializeObject(item));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<IFoodItem>(response);
                    if (string.IsNullOrEmpty(result.Id))
                        result.Id = item.Id;
                    if (result.OptionGroups == null)
                        result.OptionGroups = new List<IFoodOptionGroup>();
                    return result;
                }
            }
            return null;
        }
        public bool DeleteItem(IFoodItem item, string categoryId)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_categoryItemUrlFormat, _merchantId, categoryId, item.ProductId)));
                return answer.Count == 1;
            }
            return false;
        }
        #endregion

        #region options groups
        public List<IFoodOptionGroup> GetOptionGroups()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.catalog, string.Format(_optionGroupUrlFormat, _merchantId)));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<List<IFoodOptionGroup>>(response);
            }
            return null;
        }
        public IFoodOptionGroup InsertOptionGroup(IFoodOptionGroup optionGroup, string productId)
        {
            if (GetToken())
            {
                optionGroup.Id = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(optionGroup.Name))
                    optionGroup.Name = " ";

                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_optionGroupUrlFormat, _merchantId)), JsonConvert.SerializeObject(optionGroup));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var insertedOptionGroup = JsonConvert.DeserializeObject<IFoodOptionGroup>(response);
                    if (insertedOptionGroup != null)
                    {
                        answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                        response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_optionGroupProductUrlFormat, _merchantId, insertedOptionGroup.Id, productId)), JsonConvert.SerializeObject(optionGroup));
                        if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                            return insertedOptionGroup;
                    }
                }
            }
            return null;
        }
        public bool DeleteOptionGroup(IFoodOptionGroup optionGroup)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_optionGroupUrlFormat, _merchantId) + "/" + optionGroup.Id));
                return answer.Count == 1;
            }
            return false;
        }
        #endregion

        #region options
        public IFoodOption InsertOption(IFoodOption option, string groupId, string productId)
        {
            if (GetToken())
            {
                option.Id = Guid.NewGuid().ToString();
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_optionProductUrlFormat, _merchantId, groupId, productId)), JsonConvert.SerializeObject(option));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<IFoodOption>(response);
            }
            return null;
        }
        public bool DeleteOption(IFoodOption option, string groupId, string productId)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_optionProductUrlFormat, _merchantId, groupId, productId)));
                return answer.Count == 1;
            }
            return false;
        }
        #endregion

        #region pizzas
        public List<IFoodPizza> GetPizzas()
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.GET, GetUrl(UrlType.catalog, string.Format(_pizzasUrlFormat, _merchantId)));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<List<IFoodPizza>>(response);
            }
            return null;
        }
        public IFoodPizza InsertPizza(IFoodPizza pizza)
        {
            if (GetToken())
            {
                //pizza.Id = Guid.NewGuid().ToString();
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_pizzasUrlFormat, _merchantId)), JsonConvert.SerializeObject(pizza));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return JsonConvert.DeserializeObject<IFoodPizza>(response);
            }
            return null;
        }
        public IFoodPizza UpdatePizza(IFoodPizza pizza)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.PUT, GetUrl(UrlType.catalog, string.Format(_pizzasUrlFormat, _merchantId) + "/" + pizza.Id), JsonConvert.SerializeObject(pizza));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                {
                    var result = JsonConvert.DeserializeObject<IFoodPizza>(response);
                    if (string.IsNullOrEmpty(result.Id))
                        result.Id = pizza.Id;
                    return result;
                }
            }
            return null;
        }
        public bool DeletePizza(IFoodPizza pizza, string categoryId)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_pizzaCategoryUrlFormat, _merchantId, pizza.Id, categoryId)));
                return answer.Count == 1;
            }
            return false;
        }
        public bool LinkPizza(IFoodPizza pizza, string categoryId)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created };
                var response = Request(ref answer, HttpMethod.POST, GetUrl(UrlType.catalog, string.Format(_pizzaCategoryUrlFormat, _merchantId, pizza.Id, categoryId)), JsonConvert.SerializeObject(pizza));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return true;
            }
            return false;
        }
        public bool UnlinkPizza(IFoodPizza pizza, string categoryId)
        {
            if (GetToken())
            {
                var answer = new HashSet<HttpStatusCode>() { HttpStatusCode.OK };
                var response = Request(ref answer, HttpMethod.DELETE, GetUrl(UrlType.catalog, string.Format(_pizzaCategoryUrlFormat, _merchantId, pizza.Id, categoryId)), JsonConvert.SerializeObject(pizza));
                if (answer.Count == 1 && !string.IsNullOrEmpty(response))
                    return true;
            }
            return false;
        }
        #endregion

        #endregion

        #region private methos

        #region http
        private bool GetToken()
        {
            ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls | (System.Net.SecurityProtocolType)768 | (System.Net.SecurityProtocolType)3072; //Ssl3, Tls, Tls1.1 e Tls1.2
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; }; //Confia em toda a gente

            if (_token != null && _token.IsValid)
                return true;
            else
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(UrlType.authentication, _tokenUrl));
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Timeout = 60000;
                    request.Method = "POST";

                    byte[] bytes = Encoding.ASCII.GetBytes("grantType=client_credentials&clientId=" + _client_id + "&clientSecret=" + _client_secret);
                    request.ContentLength = bytes.Length;

                    using (Stream requestStream = request.GetRequestStream())
                        requestStream.Write(bytes, 0, bytes.Length);

                    DateTime tokenCreated = DateTime.Now;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response == null)
                            throw new Exception("Can not obtain response from server!");

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = response.GetResponseStream())
                            using (var reader = new StreamReader(stream))
                            {
                                var content = reader.ReadToEnd();
                                _token = JsonConvert.DeserializeObject<IFoodToken>(content);
                                _token.CreatedDate = tokenCreated;
                                return true;
                            }
                        }
                        else
                            throw new Exception("Invalid response from server!");
                    }
                }
                catch (Exception ex)
                {
                }
            }
            _token = null;
            return false;
        }
        private string Request(ref HashSet<HttpStatusCode> answser, HttpMethod method, string url, string postData = null, List<KeyValuePair<string, string>> addicionalHeaders = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("Authorization", "Bearer " + _token.AccessToken);
                request.Timeout = 30000;
                request.ContentType = "application/json;charset=UTF-8";
                request.Accept = "application/json";
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.Method = method.ToString();

                if (addicionalHeaders?.Count > 0)
                {
                    foreach (var header in addicionalHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (!string.IsNullOrEmpty(postData))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = bytes.Length;

                    using (Stream requestStream = request.GetRequestStream())
                        requestStream.Write(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response == null)
                        throw new Exception("Can not obtain response from server!");

                    if (answser.Contains(response.StatusCode))
                    {
                        answser.Clear();
                        answser.Add(response.StatusCode);
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                            return reader.ReadToEnd();
                    }
                    else
                    {
                        answser.Clear();
                        throw new Exception("Invalid response from server!");
                    }
                }
            }
            catch (Exception ex)
            {
                answser.Clear();
            }
            return null;
        }
        #endregion

        #region utils
        private string GetUrl(UrlType type, string endpoint) => string.Format(_baseUrlFormat, type) + endpoint;
        private List<T> CreateList<T>(T element) => new List<T>();
        #endregion

        #endregion
    }
}
