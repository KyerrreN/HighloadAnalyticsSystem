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
    http_req_duration: ["p(95)<50"],
  },
};

export default function () {
  const url = "http://localhost:5209/api/v1/events";

  const randomActorId = "user-" + Math.random().toString(36).substring(2, 10);
  const randomSessionId =
    "session-" + Math.random().toString(36).substring(2, 10);

  const payload = JSON.stringify({
    eventId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    projectApiKey: "load-test-key-123",
    timestamp: new Date().toISOString(),
    eventName: "UserClickedButton",
    actorId: randomActorId,
    sessionId: randomSessionId,
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
