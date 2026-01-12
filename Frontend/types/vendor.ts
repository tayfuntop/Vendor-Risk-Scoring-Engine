export interface DocumentStatus {
  contractValid: boolean;
  privacyPolicyValid: boolean;
  pentestReportValid: boolean;
}

export interface Vendor {
  id: number;
  name: string;
  financialHealth: number;
  slaUptime: number;
  majorIncidents: number;
  securityCerts: string[];
  documents: DocumentStatus;
}

export interface CreateVendorRequest {
  name: string;
  financialHealth: number;
  slaUptime: number;
  majorIncidents: number;
  securityCerts: string[];
  documents: DocumentStatus;
}

export interface RiskAssessment {
  vendorId: number;
  riskScore: number;
  riskLevel: string;
  reason: string;
}

export interface VendorRiskComparisonItem {
  id: number;
  name: string;
  overallRiskScore: number;
  riskLevel: string;
  rank: number;
  strengths: string[];
  weaknesses: string[];
}

export interface ComparisonSummary {
  lowestRiskVendor: string;
  highestRiskVendor: string;
  averageRiskScore: number;
  riskLevelDistribution: { [key: string]: number };
  commonRisks: string[];
}

export interface VendorComparison {
  vendors: VendorRiskComparisonItem[];
  summary: ComparisonSummary;
  recommendation: string;
}

export interface CompareVendorsRequest {
  vendorIds: number[];
}
