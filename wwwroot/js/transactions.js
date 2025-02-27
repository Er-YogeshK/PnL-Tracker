document.addEventListener("DOMContentLoaded", function () {
    const modal = new bootstrap.Modal(document.getElementById("transactionModal"));
    const form = document.getElementById("transactionForm");

    document.getElementById("openModalBtn").addEventListener("click", function () {
        document.getElementById("transactionId").value = "";
        form.reset();
        modal.show();
    });

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
        if (response.ok) location.reload();
        else alert(data.message);
    };

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

    document.querySelectorAll(".deleteBtn").forEach(button => {
        button.addEventListener("click", async function () {
            if (confirm("Are you sure?")) {
                await fetch(`/Transactions/DeleteTransaction?id=${this.dataset.id}`, { method: "GET" });
                location.reload();
            }
        });
    });
});
