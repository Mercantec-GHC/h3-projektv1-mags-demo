import { writable } from "svelte/store";
import { jwtDecode } from "jwt-decode";

function createAuthStore() {
  const { subscribe, set, update } = writable({
    isAuthenticated: false,
    username: "",
  });

  return {
    subscribe,
    login: (token) => {
      localStorage.setItem("token", token);
      const decodedToken = jwtDecode(token);
      const username =
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        ];
      set({ isAuthenticated: true, username });
    },
    logout: () => {
      localStorage.removeItem("token");
      set({ isAuthenticated: false, username: "" });
    },
    init: () => {
      const token = localStorage.getItem("token");
      if (token) {
        const decodedToken = jwtDecode(token);
        const username =
          decodedToken[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
          ];
        set({ isAuthenticated: true, username });
      } else {
        set({ isAuthenticated: false, username: "" });
      }
    },
  };
}

export const auth = createAuthStore();
