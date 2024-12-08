using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TCViettelFC_API.Dtos.OrderTicket;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Implementations;

namespace TCViettelFCTest.UnitTest
{

    [TestFixture]
    public class TicketOrderTest_QuanTB
    {
        private DbContextOptions<Sep490G53Context> _options;
        private Sep490G53Context _context;
        private TicketOrderRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<Sep490G53Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new Sep490G53Context(_options);
            SeedData();
            _repository = new TicketOrderRepository(_context, null, null);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedData()
        {
            _context.CustomersAccounts.Add(new CustomersAccount
            {
                CustomerId = 1,
                Email = "existingcustomer@gmail.com",
                Phone = "0378793050",
                Password = "123456",
                Status = 1
            });
            _context.Matches.Add(new Match
            {
                Id = 1,
                Status = 1
            });
            _context.Areas.Add(new Area
            {
                Id = 24,
                Stands = "A",
                Floor = "2",
                Section = "1",
                Price = 50000,
                Status = 1
            });
            _context.Areas.Add(new Area
            {
                Id = 12,
                Stands = "A",
                Floor = "2",
                Section = "2",
                Price = 50000,
                Status = 1
            });
            _context.MatchAreaTickets.Add(new MatchAreaTicket
            {
                MatchId = 1,
                AreaId = 24,
                AvailableSeats = 10
            });

            _context.MatchAreaTickets.Add(new MatchAreaTicket
            {
                MatchId = 2,
                AreaId = 12,
                AvailableSeats = 5
            });

            _context.SaveChanges();
        }




        #region Test Cases

        // Test Case 1
        [Test]
        public async Task AddOrderedTicket_Guest_Success()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378793050",
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                            ItemId = 1,
                            Quantity = 1,
                            Price = 50000
                    }

                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var result = await _repository.AddOderedTicket(ticketOrderDto);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.OrderId);
        }

        // Test Case 2
        [Test]
        public async Task AddOrderedTicket_ExistingCustomer_Success()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,
                TotalAmount = 100000,
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },


                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 100000,
                    PaymentGateway = "VNPAY"
                }
            };

            var result = await _repository.AddOderedTicket(ticketOrderDto, customerId: 1);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.OrderId);
        }

        // Test Case 3
        [Test]
        public async Task AddOrderedTicket_Guest_WithSupplementaryItems_Success()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                TotalAmount = 200000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378793050",
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 2,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 200000,
                    PaymentGateway = "VNPAY"
                }
            };

            var result = await _repository.AddOderedTicket(ticketOrderDto);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.OrderId);
        }

        // Test Case 4
        [Test]
        public async Task AddOrderedTicket_ExistingCustomer_WithSupplementaryItems_Success()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,
                TotalAmount = 200000,
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 2,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 200000,
                    PaymentGateway = "VNPAY"
                }
            };

            var result = await _repository.AddOderedTicket(ticketOrderDto, customerId: 1);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.OrderId);
        }

        // Test Case 5
        [Test]
        public void AddOrderedTicket_Guest_MissingCustomerInfo_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "",
                    Phone = "0378793050",
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 1,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }

        // Test Case 6
        [Test]
        public void AddOrderedTicket_NoTickets_ThrowsException()
        {

            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,

                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }

        // Test Case 7
        [Test]
        public void AddOrderedTicket_IncompleteTicketInfo_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,
                TotalAmount = 150000,

                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = null,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 1,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }

        [Test]
        public void AddOrderedTicket_AreaFull_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                            ItemId = 1,
                            Quantity = 1,
                            Price = 50000
                    }

                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            // Giả lập khu vực đã hết chỗ
            var area = _context.MatchAreaTickets.First(x => x.MatchId == 1 && x.AreaId == 24);
            area.AvailableSeats = 0;
            _context.SaveChanges();

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_MissingOrderTicketId_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,
                TotalAmount = 150000,
                OrderedTickets = new List<OrderedTicketDto>
                {
                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 1,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = null, // Thiếu OrderTicketId
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_MissingTotalAmount_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {

                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = null, // Thiếu TotalAmount
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }

        [Test]
        public async Task AddOrderedTicket_MultipleTickets_Success()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                TotalAmount = 200000,
                OrderedTickets = new List<OrderedTicketDto>
        {

            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            },
            new OrderedTicketDto
            {
                MatchId = 2,
                AreaId = 12,
                Price = 100000
            }
        },
                OrderedSuppItems = null,
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 200000,
                    PaymentGateway = "VNPAY"
                }
            };

            var result = await _repository.AddOderedTicket(ticketOrderDto, customerId: 1);

            Assert.NotNull(result);
            Assert.AreEqual(1, result.OrderId);
        }
        [Test]
        public void AddOrderedTicket_TotalAmountMismatch_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                TotalAmount = 100000,
                OrderedTickets = new List<OrderedTicketDto>
            {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
            },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 50000, // Tổng tiền không khớp
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_InvalidEmail_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quan@.com", // Email sai định dạng
                    Phone = "0378793050",
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 1,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }


        [Test]
        public void AddOrderedTicket_MissingPaymentInfo_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = 1,
                TotalAmount = 100000,
                OrderedTickets = new List<OrderedTicketDto>
        {

            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = null, // Không có phụ kiện
                PaymentDto = null // Thiếu thông tin thanh toán
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: 1));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_CustomerIdAndAddCustomerDtoIsNull_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = null,
                OrderedTickets = new List<OrderedTicketDto>
                {

                    new OrderedTicketDto
                    {
                        MatchId = 1,
                        AreaId = 24,
                        Price = 100000
                    }
                },

                OrderedSuppItems = new List<OrderedSuppItemDto>
                {
                    new OrderedSuppItemDto
                    {
                        ItemId = 1,
                        Quantity = 1,
                        Price = 50000
                    }
                },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);

        }
        [Test]
        public void AddOrderedTicket_PhoneContainsSpace_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378 793 050",
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_PhoneContainsSpecialCharacter_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378-793-050@", // Phone contains special character '@'
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_FullNameContainsLeadingOrTrailingSpaces_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378793050",
                    FullName = "  Thái Bá Quân "
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_FullNameContainsSpecialCharacter_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378793050",
                    FullName = "Thái Bá Quân@" // Full name contains special character '@'
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_PhoneIsNullOrEmpty_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "", // Phone is empty
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_PhoneContainsOnlySpace_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = " ", // Phone contains only a space
                    FullName = "Thái Bá Quân"
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }
        [Test]
        public void AddOrderedTicket_FullNameContainsOnlySpace_ThrowsException()
        {
            var ticketOrderDto = new TicketOrderDto
            {
                CustomerId = null,
                TotalAmount = 150000,
                AddCustomerDto = new AddCustomerDto
                {
                    Email = "quanthai111202@gmail.com",
                    Phone = "0378793050",
                    FullName = " " // Full name contains only a space
                },
                OrderedTickets = new List<OrderedTicketDto>
        {
            new OrderedTicketDto
            {
                MatchId = 1,
                AreaId = 24,
                Price = 100000
            }
        },
                OrderedSuppItems = new List<OrderedSuppItemDto>
        {
            new OrderedSuppItemDto
            {
                ItemId = 1,
                Quantity = 1,
                Price = 50000
            }
        },
                PaymentDto = new PaymentTicketDto
                {
                    OrderTicketId = 1,
                    OrderProductId = null,
                    TotalAmount = 150000,
                    PaymentGateway = "VNPAY"
                }
            };

            var ex = Assert.ThrowsAsync<Exception>(() => _repository.AddOderedTicket(ticketOrderDto, customerId: null));

            Assert.AreEqual("Có lỗi xảy ra khi lưu thay đổi của thực thể. Xem chi tiết lỗi bên trong để biết thêm.", ex.Message);
        }


    }

    #endregion
}