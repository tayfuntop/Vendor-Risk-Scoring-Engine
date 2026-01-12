"use client";

import { Vendor } from "@/types/vendor";
import {
  Cancel as CancelIcon,
  CheckCircle as CheckCircleIcon,
} from "@mui/icons-material";
import {
  Box,
  Card,
  CardContent,
  Chip,
  Divider,
  Grid,
  Stack,
  Typography,
} from "@mui/material";
import React from "react";

interface VendorDetailsProps {
  vendor: Vendor | null;
}

const VendorDetails: React.FC<VendorDetailsProps> = ({ vendor }) => {
  if (!vendor) {
    return (
      <Card>
        <CardContent>
          <Typography
            variant="body2"
            color="text.secondary"
            align="center"
            sx={{ py: 4 }}
          >
            Select a vendor from the list to view details
          </Typography>
        </CardContent>
      </Card>
    );
  }

  const getHealthColor = (value: number) => {
    if (value >= 80) return "success";
    if (value >= 50) return "warning";
    return "error";
  };

  const getUptimeColor = (value: number) => {
    if (value >= 95) return "success";
    if (value >= 90) return "warning";
    return "error";
  };

  const getIncidentColor = (value: number) => {
    if (value === 0) return "success";
    if (value < 3) return "warning";
    return "error";
  };

  return (
    <Card>
      <CardContent>
        <Box sx={{ mb: 3 }}>
          <Typography variant="h5" gutterBottom fontWeight={600}>
            {vendor.name}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Vendor ID: {vendor.id}
          </Typography>
        </Box>

        <Divider sx={{ my: 2 }} />

        <Grid container spacing={3}>
          <Grid size={{ xs: 12, md: 4 }}>
            <Box>
              <Typography
                variant="caption"
                color="text.secondary"
                gutterBottom
                display="block"
              >
                Financial Health
              </Typography>
              <Chip
                label={`${vendor.financialHealth}/100`}
                color={getHealthColor(vendor.financialHealth)}
                sx={{ mt: 1, fontWeight: 600 }}
              />
            </Box>
          </Grid>

          <Grid size={{ xs: 12, md: 4 }}>
            <Box>
              <Typography
                variant="caption"
                color="text.secondary"
                gutterBottom
                display="block"
              >
                SLA Uptime
              </Typography>
              <Chip
                label={`${vendor.slaUptime}%`}
                color={getUptimeColor(vendor.slaUptime)}
                sx={{ mt: 1, fontWeight: 600 }}
              />
            </Box>
          </Grid>

          <Grid size={{ xs: 12, md: 4 }}>
            <Box>
              <Typography
                variant="caption"
                color="text.secondary"
                gutterBottom
                display="block"
              >
                Major Incidents
              </Typography>
              <Chip
                label={vendor.majorIncidents}
                color={getIncidentColor(vendor.majorIncidents)}
                sx={{ mt: 1, fontWeight: 600 }}
              />
            </Box>
          </Grid>

          <Grid size={{ xs: 12 }}>
            <Divider />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <Typography variant="subtitle2" gutterBottom fontWeight={600}>
              Security Certifications
            </Typography>
            <Stack direction="row" spacing={1} flexWrap="wrap" sx={{ mt: 1 }}>
              {vendor.securityCerts.length > 0 ? (
                vendor.securityCerts.map((cert) => (
                  <Chip
                    key={cert}
                    label={cert}
                    color="primary"
                    variant="outlined"
                    sx={{ mb: 1 }}
                  />
                ))
              ) : (
                <Typography variant="body2" color="text.secondary">
                  No security certifications
                </Typography>
              )}
            </Stack>
          </Grid>

          <Grid size={{ xs: 12 }}>
            <Divider />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <Typography
              variant="subtitle2"
              gutterBottom
              fontWeight={600}
              sx={{ mb: 2 }}
            >
              Compliance Documents
            </Typography>
            <Stack spacing={1.5}>
              <Box display="flex" alignItems="center" gap={1}>
                {vendor.documents.contractValid ? (
                  <CheckCircleIcon color="success" fontSize="small" />
                ) : (
                  <CancelIcon color="error" fontSize="small" />
                )}
                <Typography variant="body2">
                  Contract:{" "}
                  {vendor.documents.contractValid ? "Valid" : "Invalid/Expired"}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" gap={1}>
                {vendor.documents.privacyPolicyValid ? (
                  <CheckCircleIcon color="success" fontSize="small" />
                ) : (
                  <CancelIcon color="error" fontSize="small" />
                )}
                <Typography variant="body2">
                  Privacy Policy:{" "}
                  {vendor.documents.privacyPolicyValid
                    ? "Valid"
                    : "Invalid/Expired"}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" gap={1}>
                {vendor.documents.pentestReportValid ? (
                  <CheckCircleIcon color="success" fontSize="small" />
                ) : (
                  <CancelIcon color="error" fontSize="small" />
                )}
                <Typography variant="body2">
                  Penetration Test Report:{" "}
                  {vendor.documents.pentestReportValid
                    ? "Valid"
                    : "Invalid/Expired"}
                </Typography>
              </Box>
            </Stack>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
};

export default VendorDetails;
