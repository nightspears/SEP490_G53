

const key = "keyCartSep490G53keyCartSep490G53";
//const encryptedCart = encryptData(cartItems, key);
//localStorage.setItem("cart", encryptedCart);

//const storedCart = localStorage.getItem("cart");
//const decryptedCart = decryptData(storedCart, key);



// Mã hóa giỏ hàng










function format() {
    debugger
    // Hàm định dạng tiền tệ Việt Nam
    function formatCurrency(value) {
        return parseInt(value).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }

    // Lấy tất cả các phần tử có class 'price'
    const prices = document.querySelectorAll('.price');

    // Lặp qua từng phần tử và định dạng giá trị dựa trên thuộc tính 'data-price'
    prices.forEach(function (priceElement) {
        const priceValue = priceElement.getAttribute('data-price');
        if (priceValue) {
            priceElement.innerText = formatCurrency(priceValue);  // Định dạng và hiển thị lại giá trị
        }
    });

}
