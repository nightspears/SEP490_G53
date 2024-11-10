function displayTickets() {
    const tickets = JSON.parse(localStorage.getItem('cart')) || [];
    const bodyData = document.getElementById('bodyData');
    bodyData.innerHTML = "";

    if (tickets.length === 0) {
        document.getElementById("noDataMessage").style.display = "block";
    } else {
        document.getElementById("noDataMessage").style.display = "none";
        tickets.forEach((ticket, index) => {
            const row = `
                            <tr>
                                <td><input type="checkbox" class="select-ticket" data-index="${index}" onchange="updateSelectedTotal()"></td>
                                <td>Match ${ticket.matchId} - Area ${ticket.areaId}</td>
                                <td>${ticket.quantity}</td>
                                <td>${ticket.price} VND</td>
                                <td>${(ticket.price * ticket.quantity).toFixed(0)} VND</td>
                                <td><input type="button" class="btn btn-info btn-sm" value="Xem thông tin vé" onclick="showTicketDetails(${index})"></td>
                                <td><button class="btn btn-danger btn-sm" onclick="removeTicket(${index})">Xóa</button></td>
                            </tr>
                        `;
            bodyData.insertAdjacentHTML("beforeend", row);
        });
    }
    updateSelectedTotal();
}

function toggleSelectAll(selectAllCheckbox) {
    const checkboxes = document.querySelectorAll('.select-ticket');
    checkboxes.forEach(checkbox => checkbox.checked = selectAllCheckbox.checked);
    updateSelectedTotal();
}

function showTicketDetails(index) {
    const tickets = JSON.parse(localStorage.getItem('cart')) || [];
    const ticket = tickets[index];

    const supplementaryDetails = ticket.supplementaryItems
        .map(item => `${item.itemId} - Số lượng: ${item.itemQuantity}, Giá: ${item.itemPrice} VND`)
        .join('<br>');

    const detailsContent = `
                    <p><strong>Cửa:</strong> ${ticket.section}</p>
                    <p><strong>Tầng:</strong> ${ticket.floor}</p>
                    <p><strong>Khán đài:</strong> ${ticket.stands}</p>
                    <p><strong>Đồ đính kèm:</strong><br>${supplementaryDetails || 'Không có'}</p>
                `;

    document.getElementById('ticketDetailsContent').innerHTML = detailsContent;
    document.getElementById('ticketDetailsModal').style.display = 'block';
}

function closeTicketDetails() {
    document.getElementById('ticketDetailsModal').style.display = 'none';
}

function updateSelectedTotal() {
    const selectedTickets = [];
    document.querySelectorAll('.select-ticket:checked').forEach(checkbox => {
        const index = checkbox.getAttribute('data-index');
        const tickets = JSON.parse(localStorage.getItem('cart')) || [];
        selectedTickets.push(tickets[index]);
    });

    let total = selectedTickets.reduce((sum, ticket) => {
        const ticketTotal = ticket.price * ticket.quantity;
        const supplementaryTotal = (ticket.supplementaryItems || []).reduce((suppSum, item) => suppSum + (item.itemPrice * item.itemQuantity), 0);
        return sum + ticketTotal + supplementaryTotal;
    }, 0);

    const vat = total * 0.05;
    const totalBill = total + vat;

    document.getElementById('totalPrice').textContent = `${total.toFixed(0)} VNĐ`;
    document.getElementById('VAT_Price').textContent = `${vat.toFixed(0)} VNĐ`;
    document.getElementById('totalBill').textContent = `${totalBill.toFixed(0)} VNĐ`;
}

function checkOutSelectedTickets() {
    const selectedTickets = [];
    const selectedIndexes = [];

    document.querySelectorAll('.select-ticket:checked').forEach(checkbox => {
        const index = parseInt(checkbox.getAttribute('data-index'), 10);
        const tickets = JSON.parse(localStorage.getItem('cart')) || [];
        selectedTickets.push(tickets[index]);
        selectedIndexes.push(index);
    });

    if (selectedTickets.length === 0) {
        alert("Vui lòng chọn ít nhất một vé để thanh toán.");
        return;
    }

    let total = selectedTickets.reduce((sum, ticket) => {
        const ticketTotal = ticket.price * ticket.quantity;
        const supplementaryTotal = (ticket.supplementaryItems || []).reduce((suppSum, item) => suppSum + (item.itemPrice * item.itemQuantity), 0);
        return sum + ticketTotal + supplementaryTotal;
    }, 0);

    const vat = total * 0.05;
    const totalBill = total + vat;

    const confirmation = confirm(`Tổng tiền cho các vé đã chọn: ${totalBill.toFixed(0)} VNĐ (bao gồm VAT). Bạn có muốn tiếp tục thanh toán không?`);
    if (!confirmation) {
        return;
    }

    const customerId = document.getElementById('cusId').value;
    const orderDate = new Date().toISOString();

    const payload = {
        CustomerId: customerId,
        OrderDate: orderDate,
        TotalAmount: totalBill,
        OrderedTickets: selectedTickets.map(ticket => ({
            MatchId: ticket.matchId,
            AreaId: ticket.areaId,
            Price: ticket.price,
            Status: 0,
            OrderId: 0
        })),
        OrderedSuppItems: selectedTickets.flatMap(ticket =>
            (ticket.supplementaryItems || []).map(item => ({
                OrderId: 0,
                ItemId: item.itemId,
                Quantity: item.itemQuantity,
                Price: item.itemPrice
            }))
        )
    };

    // Send the payload to the controller
    fetch('/CartTicket/Checkout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert(data.message);
                // Update cart after successful checkout
                const tickets = JSON.parse(localStorage.getItem('cart')) || [];
                const remainingTickets = tickets.filter((_, index) => !selectedIndexes.includes(index));
                localStorage.setItem('cart', JSON.stringify(remainingTickets));
                displayTickets();
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            console.error("Lỗi khi thanh toán:", error);
            alert("Có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.");
        });
}


document.addEventListener('DOMContentLoaded', displayTickets);