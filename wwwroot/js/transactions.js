document.addEventListener("DOMContentLoaded", function () {
    const modal = new bootstrap.Modal(document.getElementById("transactionModal"));
    const form = document.getElementById("transactionForm");

    // Get selected year from the dropdown (if exists)
    const yearFilter = document.getElementById("yearFilter");
    let selectedYear = yearFilter ? yearFilter.value : "";

    // Open Modal for Adding Transaction
    document.getElementById("openModalBtn").addEventListener("click", function () {
        document.getElementById("transactionId").value = "";
        form.reset();
        modal.show();
    });

    // Handle Form Submission (Add / Update)
    form.onsubmit = async function (event) {
        event.preventDefault();
        const id = document.getElementById("transactionId").value;
        const date = document.getElementById("date").value;
        const deposit = parseFloat(document.getElementById("deposit").value) || 0;
        const withdrawal = parseFloat(document.getElementById("withdrawal").value) || 0;

        const payload = { id, date, deposit, withdrawal };

        const response = await fetch(id ? "/Transactions/UpdateTransaction" : "/Transactions/AddTransaction", {
            method: id ? "PUT" : "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });

        const data = await response.json();
        if (response.ok) {
            location.href = selectedYear ? `/Transactions?year=${selectedYear}` : "/Transactions";
        } else {
            alert(data.message);
        }
    };

    // Edit Transaction
    document.querySelectorAll(".editBtn").forEach(button => {
        button.addEventListener("click", function () {
            const row = this.closest("tr").children;
            document.getElementById("transactionId").value = row[0].textContent;
            document.getElementById("date").value = row[1].textContent;
            document.getElementById("deposit").value = row[2].textContent;
            document.getElementById("withdrawal").value = row[3].textContent;
            modal.show();
        });
    });

    // Delete Transaction
    document.querySelectorAll(".deleteBtn").forEach(button => {
        button.addEventListener("click", async function () {
            if (confirm("Are you sure?")) {
                await fetch(`/Transactions/DeleteTransaction?id=${this.dataset.id}`, { method: "GET" });
                location.href = selectedYear ? `/Transactions?year=${selectedYear}` : "/Transactions";
            }
        });
    });

    // Handle Year Filtering
    yearFilter.addEventListener("change", function () {
        let year = this.value;
        location.href = year ? `/Transactions?year=${year}` : "/Transactions";
    });
});
