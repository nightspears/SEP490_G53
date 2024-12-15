// buyTicket.js

// Hiển thị và ẩn overlay cho các mục bổ sung
document.getElementById("showSupplementaryItems").addEventListener("click", function () {
    document.getElementById("supplementaryOverlay").style.display = "flex";
});

document.getElementById("closeOverlay").addEventListener("click", function () {
    document.getElementById("supplementaryOverlay").style.display = "none";
});

// Lấy chi tiết khu vực (area) theo ID và hiển thị trong form
function getAreaById(id) {
    fetch(`https://tcvtfcapi.azurewebsites.net/api/Area/getareabyid/${id}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('areaId').value = data.id || '';
            document.getElementById('stands').value = data.stands || '';
            document.getElementById('floor').value = data.floor || '';
            document.getElementById('section').value = data.section || '';
            document.getElementById('price').value = data.price || '';
            document.getElementById('status').value = data.status === 1 ? "Còn vé" : "Hết vé";
            document.getElementById('ticketForm').style.display = 'block';
        })
        .catch(error => console.error('Error fetching data:', error));
}

// Hiển thị chi tiết trận đấu
function loadMatchDetails() {
    const matchId = '@Context.Request.Query["matchId"]';
    if (!matchId) {
        console.error("Match ID is missing.");
        document.getElementById('matchDetails').textContent = 'Match ID not provided.';
        return;
    }

    fetch(`https://tcvtfcapi.azurewebsites.net/api/Matches/GetMatchesById?id=${matchId}`)
        .then(response => response.json())
        .then(data => {
            const opponentName = data.opponentName || 'Unknown Opponent';
            const matchDate = new Date(data.matchDate);
            const formattedDate = matchDate.toLocaleDateString('vi-VN', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
            document.getElementById('matchDetails').textContent = `Viettel FC vs ${opponentName} || ${formattedDate}`;
        })
        .catch(error => {
            console.error('Error fetching match details:', error);
            document.getElementById('matchDetails').textContent = 'Failed to load match details.';
        });
}

document.addEventListener('DOMContentLoaded', () => {
    loadMatchDetails();
    const paths = document.querySelectorAll('[id^="path-"]');
    paths.forEach(path => {
        path.addEventListener('click', () => {
            const areaId = path.id.replace('path-', '');
            getAreaById(areaId);
        });
    });
});

// Cập nhật tổng giá khi thay đổi số lượng hoặc khi chọn đồ đính kèm
function updateTotalPrice() {
    const pricePerTicket = parseFloat(document.getElementById('price').value) || 0;
    const quantity = parseInt(document.getElementById('quantity').value) || 0;
    let supplementaryTotal = 0;

    document.querySelectorAll('input[name="supplementary[]"]:checked').forEach(item => {
        supplementaryTotal += parseFloat(item.getAttribute('data-price')) || 0;
    });

    const totalPrice = (pricePerTicket * quantity) + supplementaryTotal;
    document.getElementById('totalPrice').value = totalPrice.toFixed(0) + ' VND';
}

document.getElementById('quantity').addEventListener('change', updateTotalPrice);
document.querySelectorAll('input[name="supplementary[]"]').forEach(item => {
    item.addEventListener('change', updateTotalPrice);
});

document.addEventListener('DOMContentLoaded', function () {
    updateTotalPrice();
});

// Thêm vé vào giỏ hàng
document.getElementById('addToCartBtn').addEventListener('click', function () {
    const customerId = document.getElementById('cusId').value;
    if (!customerId) {
        alert("Vui lòng đăng nhập để sử dụng chức năng này.");
        return;
    }

    const ticket = {
        matchId: document.getElementById('matchId').value,
        areaId: document.getElementById('areaId').value,
        stands: document.getElementById('stands').value,
        floor: document.getElementById('floor').value,
        section: document.getElementById('section').value,
        price: document.getElementById('price').value,
        status: document.getElementById('status').value,
        quantity: document.getElementById('quantity').value,
        supplementaryItems: []
    };

    document.querySelectorAll('input[name="supplementary[]"]:checked').forEach(item => {
        const itemId = parseInt(item.value);
        const itemPrice = parseFloat(item.getAttribute('data-price'));
        const itemQuantity = parseInt(document.querySelector(`input[name="quantity_${itemId}"]`).value);
        ticket.supplementaryItems.push({ itemId, itemPrice, itemQuantity });
    });

    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    cart.push(ticket);
    localStorage.setItem('cart', JSON.stringify(cart));
    alert("Đã thêm vào giỏ hàng thành công.");
});

// Xử lý đặt vé và gửi dữ liệu lên API
document.querySelector('form#ticketForm').addEventListener('submit', async (event) => {
    event.preventDefault();

    const payload = {
        addCustomerDto: null,
        orderDate: new Date().toISOString(),
        totalAmount: parseFloat(document.getElementById('totalPrice').value) || 0,
        customerId: parseInt(document.getElementById('cusId').value) || null,
        orderedTickets: Array.from({ length: parseInt(document.getElementById('quantity').value) }, () => ({
            matchId: parseInt(document.getElementById('matchId').value),
            areaId: parseInt(document.getElementById('areaId').value),
            price: parseFloat(document.getElementById('price').value),
            status: document.getElementById('status').value === "Available" ? 1 : 0
        })),
        orderedSuppItems: Array.from(document.querySelectorAll('input[name="supplementary[]"]:checked')).map(item => ({
            itemId: parseInt(item.value),
            quantity: parseInt(document.querySelector(`input[name="quantity_${item.value}"]`).value),
            price: parseFloat(item.getAttribute('data-price'))
        }))
    };

    const url = payload.customerId
        ? `https://tcvtfcapi.azurewebsites.net/api/TicketOrder?customerId=${payload.customerId}`
        : `https://tcvtfcapi.azurewebsites.net/api/TicketOrder`;

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error("Đặt vé không thành công.");

        document.getElementById('successPopup').style.display = 'flex';
        setTimeout(() => {
            document.getElementById('successPopup').style.display = 'none';
        }, 3000);

    } catch (error) {
        console.error("Lỗi:", error);
        alert("Đã xảy ra lỗi trong quá trình đặt vé. Vui lòng thử lại.");
    }
});
