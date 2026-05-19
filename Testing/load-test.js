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

export default function () {
  const url = "http://localhost:5209/api/v1/events";

  const payload = JSON.stringify({
    eventId: "77a85f64-5117-4562-b3fc-2c963f66afa6",
    projectApiKey: "prod-live-key-777",
    timestamp: new Date().toISOString(),
    eventName: "AppHeartbeat",
    userId: "user-active-111",
    sessionId: "session-xyz",
    properties: { os: "Android", app_version: "2.4.1" },
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
