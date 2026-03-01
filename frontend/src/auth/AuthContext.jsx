import React, { createContext, useContext, useMemo, useState } from "react";
import { http } from "../api/httpClient";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [role, setRole] = useState(null);
  const [identifier, setIdentifier] = useState(null);
  const [accessToken, setAccessToken] = useState(localStorage.getItem("accessToken"));
  const [refreshToken, setRefreshToken] = useState(localStorage.getItem("refreshToken"));

  const isLoggedIn = !!accessToken;

  const startOtp = async (identifierInput, channel) => {
    if (!role) throw new Error("Role not selected.");
    const trimmed = identifierInput.trim();
    if (!trimmed) throw new Error("Identifier required.");

    setIdentifier(trimmed);

    await http.post("/api/auth/start", {
      identifier: trimmed,
      channel,
      role,
    });
  };

  const verifyOtp = async (otp, firstName, fullName) => {
    if (!identifier || !role) throw new Error("Missing auth context.");

    const res = await http.post("/api/auth/verify", {
      identifier,
      otp: otp.trim(),
      role,
      firstName: firstName || null,
      fullName: fullName || null,
    });

    const { accessToken, refreshToken } = res.data;

    localStorage.setItem("accessToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);

    setAccessToken(accessToken);
    setRefreshToken(refreshToken);
  };

  const logout = async () => {
    const storedRefresh = localStorage.getItem("refreshToken");

    if (storedRefresh) {
      try {
        await http.post("/api/auth/logout", { refreshToken: storedRefresh });
      } catch {}
    }

    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");

    setAccessToken(null);
    setRefreshToken(null);
  };

  const value = useMemo(
    () => ({
      role,
      setRole,
      identifier,
      accessToken,
      refreshToken,
      isLoggedIn,
      startOtp,
      verifyOtp,
      logout,
    }),
    [role, identifier, accessToken, refreshToken]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider");
  return context;
}