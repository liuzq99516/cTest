(function () {
    var hints = [
        { match: /\/jobs\/failed/i, text: "当前没有失败的作业。" },
        { match: /\/jobs\/enqueued/i, text: "当前没有排队中的作业。" },
        { match: /\/jobs\/processing/i, text: "当前没有正在执行的作业。" },
        { match: /\/jobs\/succeeded/i, text: "当前没有已完成的作业。" },
        { match: /\/jobs\/deleted/i, text: "当前没有被删除的作业。" },
        { match: /\/jobs\/scheduled/i, text: "当前没有计划中的作业。" },
        { match: /\/jobs\/awaiting/i, text: "当前没有等待中的作业。" },
        { match: /\/recurring/i, text: "当前没有周期性作业。启用 JavaPush:EnableBackgroundPush 后会注册「成绩推送重试」任务。" },
        { match: /\/retries/i, text: "当前没有待重试的作业。" },
        { match: /\/queues/i, text: "当前不存在任务队列。向 Hangfire 入队后，作业会出现在 default 队列。" },
        { match: /\/servers/i, text: "当前没有活动的服务器。请确认 API 进程已启动且 Hangfire Server 已注册。" }
    ];

    function getContentRoot() {
        return document.querySelector("#content .col-md-10")
            || document.querySelector("#content")
            || document.querySelector(".container .row > div:last-child");
    }

    function resolveMessage(path) {
        for (var i = 0; i < hints.length; i++) {
            if (hints[i].match.test(path)) {
                return hints[i].text;
            }
        }
        return "当前分类下暂无作业数据。";
    }

    function hasDataRows(root) {
        var rows = root.querySelectorAll("table tbody tr");
        return rows.length > 0;
    }

    function getVisibleAlert(root) {
        var alerts = root.querySelectorAll(".alert:not(.hangfire-empty-hint)");
        for (var i = 0; i < alerts.length; i++) {
            var text = (alerts[i].textContent || "").replace(/\s+/g, " ").trim();
            if (text.length > 0 && alerts[i].offsetHeight > 0) {
                return alerts[i];
            }
        }
        return null;
    }

    function ensureEmptyHint() {
        var root = getContentRoot();
        if (!root) {
            return;
        }

        if (hasDataRows(root)) {
            return;
        }

        var visibleAlert = getVisibleAlert(root);
        if (visibleAlert) {
            return;
        }

        var existing = root.querySelector(".hangfire-empty-hint");
        if (existing) {
            return;
        }

        var div = document.createElement("div");
        div.className = "alert alert-info hangfire-empty-hint";
        div.setAttribute("role", "status");
        div.innerHTML = "<strong>提示：</strong>" + resolveMessage(location.pathname);

        var header = root.querySelector("h1.page-header");
        if (header) {
            header.insertAdjacentElement("afterend", div);
        } else {
            root.prepend(div);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", ensureEmptyHint);
    } else {
        ensureEmptyHint();
    }
})();
