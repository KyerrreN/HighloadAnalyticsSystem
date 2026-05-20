import http from "k6/http";
import { check } from "k6";

export const options = {
  stages: [
    { duration: "10s", target: 500 },
    { duration: "500s", target: 500 },
    { duration: "10s", target: 0 },
  ],
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<5"],
  },
};

export default function () {
  const url = "http://localhost:5209/api/v1/events";

  const payload = JSON.stringify({
    eventId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    projectApiKey: "load-test-key-123",
    timestamp: new Date().toISOString(),
    eventName: "UserClickedButton",
    userId: "test-user-999",
    sessionId: "session-abc",
    properties: { buttonColor: "red", loadTest: true },
  });

  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const response = http.post(url, payload, params);

  check(response, {
    "is status 202": (r) => r.status === 202,
    "is channel full (503/429)": (r) => r.status === 503 || r.status === 429,
  });
}
