import {
  CompareVendorsRequest,
  RiskAssessment,
  Vendor,
  VendorComparison,
} from "@/types/vendor";
import axios from "axios";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

const api = {
  // Get all vendors
  getVendors: async (): Promise<Vendor[]> => {
    const response = await apiClient.get<{ vendors: Vendor[] }>("/vendor");
    return response.data.vendors;
  },

  // Get vendor by ID
  getVendorById: async (id: number): Promise<Vendor> => {
    const response = await apiClient.get<{ vendor: Vendor }>(`/vendor/${id}`);
    return response.data.vendor;
  },

  // Create new vendor
  createVendor: async (
    vendor: Partial<Vendor>
  ): Promise<{ id: number; name: string }> => {
    const response = await apiClient.post<{ id: number; name: string }>(
      "/vendor",
      vendor
    );
    return response.data;
  },

  // Get risk assessment for a vendor
  getVendorRisk: async (id: number): Promise<RiskAssessment> => {
    const response = await apiClient.get<{ assessment: RiskAssessment }>(
      `/vendor/${id}/risk`
    );
    return response.data.assessment;
  },

  // Compare vendors
  compareVendors: async (vendorIds: number[]): Promise<VendorComparison> => {
    const request: CompareVendorsRequest = { vendorIds };
    const response = await apiClient.post<{ comparison: VendorComparison }>(
      "/vendor/compare",
      request
    );
    return response.data.comparison;
  },
};

export const vendorApi = api;
export default api;
