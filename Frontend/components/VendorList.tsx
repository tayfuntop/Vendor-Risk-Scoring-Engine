'use client';

import React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  IconButton,
  Tooltip,
  Typography,
  Box,
} from '@mui/material';
import {
  Visibility as VisibilityIcon,
  Assessment as AssessmentIcon,
} from '@mui/icons-material';
import { Vendor } from '@/types/vendor';

interface VendorListProps {
  vendors: Vendor[];
  onViewDetails: (vendor: Vendor) => void;
  onViewRisk: (vendor: Vendor) => void;
}

const VendorList: React.FC<VendorListProps> = ({ vendors, onViewDetails, onViewRisk }) => {
  const getRiskColor = (financialHealth: number, slaUptime: number, incidents: number) => {
    if (financialHealth >= 80 && slaUptime >= 95 && incidents === 0) return 'success';
    if (financialHealth < 50 || slaUptime < 90 || incidents >= 3) return 'error';
    return 'warning';
  };

  return (
    <TableContainer component={Paper} sx={{ maxHeight: 600, overflowX: 'auto' }}>
      <Table stickyHeader sx={{ minWidth: { xs: 650, md: 750 } }}>
        <TableHead>
          <TableRow>
            <TableCell sx={{ display: { xs: 'none', sm: 'table-cell' } }}>
              <Typography variant="subtitle2" fontWeight={600}>ID</Typography>
            </TableCell>
            <TableCell>
              <Typography variant="subtitle2" fontWeight={600}>Name</Typography>
            </TableCell>
            <TableCell align="center" sx={{ display: { xs: 'none', md: 'table-cell' } }}>
              <Typography variant="subtitle2" fontWeight={600}>Financial Health</Typography>
            </TableCell>
            <TableCell align="center" sx={{ display: { xs: 'none', md: 'table-cell' } }}>
              <Typography variant="subtitle2" fontWeight={600}>SLA Uptime</Typography>
            </TableCell>
            <TableCell align="center" sx={{ display: { xs: 'none', lg: 'table-cell' } }}>
              <Typography variant="subtitle2" fontWeight={600}>Incidents</Typography>
            </TableCell>
            <TableCell sx={{ display: { xs: 'none', lg: 'table-cell' } }}>
              <Typography variant="subtitle2" fontWeight={600}>Security Certs</Typography>
            </TableCell>
            <TableCell align="center">
              <Typography variant="subtitle2" fontWeight={600}>Actions</Typography>
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {vendors.length === 0 ? (
            <TableRow>
              <TableCell colSpan={7} align="center">
                <Typography variant="body2" color="text.secondary" sx={{ py: 4 }}>
                  No vendors found. Create your first vendor to get started.
                </Typography>
              </TableCell>
            </TableRow>
          ) : (
            vendors.map((vendor) => (
              <TableRow
                key={vendor.id}
                hover
                sx={{ cursor: 'pointer', '&:hover': { bgcolor: 'action.hover' } }}
                onClick={() => onViewDetails(vendor)}
              >
                <TableCell sx={{ display: { xs: 'none', sm: 'table-cell' } }}>{vendor.id}</TableCell>
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>
                    {vendor.name}
                  </Typography>
                  <Box sx={{ display: { xs: 'flex', md: 'none' }, gap: 0.5, mt: 0.5, flexWrap: 'wrap' }}>
                    <Chip
                      label={`FH: ${vendor.financialHealth}`}
                      size="small"
                      color={getRiskColor(vendor.financialHealth, vendor.slaUptime, vendor.majorIncidents)}
                    />
                    <Chip
                      label={`SLA: ${vendor.slaUptime}%`}
                      size="small"
                      color={vendor.slaUptime >= 95 ? 'success' : vendor.slaUptime >= 90 ? 'warning' : 'error'}
                    />
                  </Box>
                </TableCell>
                <TableCell align="center" sx={{ display: { xs: 'none', md: 'table-cell' } }}>
                  <Chip
                    label={vendor.financialHealth}
                    size="small"
                    color={getRiskColor(vendor.financialHealth, vendor.slaUptime, vendor.majorIncidents)}
                  />
                </TableCell>
                <TableCell align="center" sx={{ display: { xs: 'none', md: 'table-cell' } }}>
                  <Chip
                    label={`${vendor.slaUptime}%`}
                    size="small"
                    color={vendor.slaUptime >= 95 ? 'success' : vendor.slaUptime >= 90 ? 'warning' : 'error'}
                  />
                </TableCell>
                <TableCell align="center" sx={{ display: { xs: 'none', lg: 'table-cell' } }}>
                  <Chip
                    label={vendor.majorIncidents}
                    size="small"
                    color={vendor.majorIncidents === 0 ? 'success' : vendor.majorIncidents < 3 ? 'warning' : 'error'}
                  />
                </TableCell>
                <TableCell sx={{ display: { xs: 'none', lg: 'table-cell' } }}>
                  <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
                    {vendor.securityCerts.length > 0 ? (
                      vendor.securityCerts.map((cert) => (
                        <Chip key={cert} label={cert} size="small" variant="outlined" />
                      ))
                    ) : (
                      <Typography variant="caption" color="text.secondary">
                        None
                      </Typography>
                    )}
                  </Box>
                </TableCell>
                <TableCell align="center">
                  <Tooltip title="View Details">
                    <IconButton
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        onViewDetails(vendor);
                      }}
                    >
                      <VisibilityIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Risk Assessment">
                    <IconButton
                      size="small"
                      color="primary"
                      onClick={(e) => {
                        e.stopPropagation();
                        onViewRisk(vendor);
                      }}
                    >
                      <AssessmentIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default VendorList;
