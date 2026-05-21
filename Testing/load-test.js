import http from "k6/http";
import { check } from "k6";

export const options = {
  scenarios: {
    telemetry_workflow: {
      executor: "ramping-arrival-rate",
      startRate: 0,
      timeUnit: "1s",
      preAllocatedVUs: 300,
      maxVUs: 2000,
      stages: [
        { duration: "15s", target: 4000 },
        { duration: "30s", target: 4000 },
        { duration: "10s", target: 20000 },
        { duration: "30s", target: 20000 },
        { duration: "15s", target: 0 },
      ],
    },
  },
  thresholds: {
    http_req_failed: ["rate<0.001"],
    http_req_duration: ["p(95)<10"],
  },
};

function generateUUID() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c === "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export default function () {
  const url = "http://localhost:5209/api/v1/events";

  const totalUniqueUsers = 50000;
  const randomActorNum = Math.floor(Math.random() * totalUniqueUsers);
  const actorId = `user-${randomActorNum}`;

  const randomSessionNum = Math.floor(Math.random() * 200000);
  const sessionId = `sess-${randomSessionNum}`;

  const eventTypes = [
    "AppHeartbeat",
    "PageView",
    "ButtonClick",
    "CheckoutStarted",
    "OrderPlaced",
  ];
  const eventName = eventTypes[Math.floor(Math.random() * eventTypes.length)];

  const osType = Math.random() > 0.5 ? "Android" : "iOS";

  const payload = JSON.stringify({
    eventId: generateUUID(),
    projectApiKey: "prod-live-key-777",
    timestamp: new Date().toISOString(),
    eventName: eventName,
    actorId: actorId,
    sessionId: sessionId,
    properties: {
      os: osType,
      app_version: "2.4.1",
    },
  });

  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const response = http.post(url, payload, params);

  check(response, {
    "status is 202": (r) => r.status === 202,
  });
}
